using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DealW.Application.Services
{
    public class MatchmakingBackgroundService(
        ILogger<MatchmakingBackgroundService> logger,
        IServiceProvider serviceProvider,
        IHubContext<Hub> duelHubContext) : BackgroundService
    {
        // Интервал запуска матчмейкинга в миллисекундах
        private const int MATCHMAKING_INTERVAL = 3000;
        // Диапазон рейтинга для поиска оппонентов
        private const int RATING_RANGE = 100;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Служба матчмейкинга запущена");
            
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await PerformMatchmakingAsync();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Ошибка в процессе матчмейкинга");
                }
                
                await Task.Delay(MATCHMAKING_INTERVAL, stoppingToken);
            }
            
            logger.LogInformation("Служба матчмейкинга остановлена");
        }
        
        /// <summary>
        /// Основной метод матчмейкинга
        /// </summary>
        private async Task PerformMatchmakingAsync()
        {
            using var scope = serviceProvider.CreateScope();
            var redisService = scope.ServiceProvider.GetRequiredService<IRedisService>();
            var duelService = scope.ServiceProvider.GetRequiredService<DuelService>();
            var userService = scope.ServiceProvider.GetRequiredService<UserService>();
            
            logger.LogDebug("Запуск цикла матчмейкинга");
            
            // Сначала проверяем, есть ли пользователи в очереди Redis
            var userIdsInQueue = await redisService.GetAllUsersInMatchmakingQueueAsync();
            
            if (userIdsInQueue.Count == 0)
            {
                logger.LogDebug("Нет пользователей в очереди матчмейкинга (Redis)");
                return;
            }
            
            logger.LogDebug($"Найдено {userIdsInQueue.Count} пользователей в очереди Redis");
            
            // Получаем список всех пользователей в очереди матчмейкинга из БД
            var searchingUsers = await userService.GetUsersSearchingOpponentsAsync();
            
            if (searchingUsers.Count == 0)
            {
                logger.LogDebug("Нет пользователей в очереди матчмейкинга (БД)");
                return;
            }
            
            logger.LogInformation($"Найдено {searchingUsers.Count} пользователей в очереди матчмейкинга");
            
            // Отслеживаем уже обработанных пользователей
            var processedUserIds = new HashSet<int>();
            
            foreach (var user in searchingUsers)
            {
                if (processedUserIds.Contains(user.Id))
                {
                    continue;
                }
                
                // Проверяем, что пользователь все еще в очереди Redis
                bool isInQueue = await redisService.IsUserInMatchmakingQueueAsync(user.Id);
                if (!isInQueue)
                {
                    continue;
                }
                
                logger.LogDebug($"Поиск оппонента для пользователя {user.Id} с рейтингом {user.Rating}");
                
                // Ищем пользователей с похожим рейтингом
                var potentialMatches = await redisService.FindMatchesInRangeAsync(user.Rating, RATING_RANGE);
                
                // Исключаем текущего пользователя и уже обработанных
                potentialMatches = potentialMatches
                    .Where(id => id != user.Id && !processedUserIds.Contains(id))
                    .ToList();
                
                if (potentialMatches.Count == 0)
                {
                    logger.LogDebug($"Нет подходящих оппонентов для пользователя {user.Id}");
                    continue;
                }
                
                // Выбираем первое совпадение
                var opponentId = potentialMatches.First();
                var opponent = searchingUsers.FirstOrDefault(u => u.Id == opponentId);
                
                if (opponent == null)
                {
                    logger.LogWarning($"Оппонент {opponentId} найден в Redis, но не найден в базе данных");
                    continue;
                }
                
                logger.LogInformation($"Найдено совпадение между пользователями {user.Id} и {opponentId}");
                
                // Удаляем обоих пользователей из очереди Redis
                var userRemoved = await redisService.RemoveUserFromMatchmakingQueueAsync(user.Id);
                var opponentRemoved = await redisService.RemoveUserFromMatchmakingQueueAsync(opponentId);
                
                if (!userRemoved || !opponentRemoved)
                {
                    logger.LogWarning($"Не удалось удалить пользователей из очереди Redis: {user.Id} ({userRemoved}), {opponentId} ({opponentRemoved})");
                    continue;
                }
                
                // Обновляем статус поиска в базе данных
                await userService.UpdateSearchStatusAsync(user.Id, false);
                await userService.UpdateSearchStatusAsync(opponentId, false);
                
                // Создаем новую дуэль
                var newDuel = await duelService.CreateDuelAsync(user.Id, opponentId);
                
                if (newDuel == null)
                {
                    logger.LogError($"Не удалось создать дуэль между пользователями {user.Id} и {opponentId}");
                    continue;
                }
                
                // Сохраняем дуэль в Redis для быстрого доступа
                await redisService.SaveActiveDuelAsync(newDuel);
                
                // Отправляем уведомления пользователям через SignalR
                try
                {
                    logger.LogInformation($"Отправка уведомления о создании дуэли {newDuel.Id} пользователям {user.Id} и {opponentId} через SignalR");
                    
                    await duelHubContext.Clients.Group($"user_{user.Id}").SendAsync("DuelCreated", newDuel.Id);
                    await duelHubContext.Clients.Group($"user_{opponentId}").SendAsync("DuelCreated", newDuel.Id);
                    
                    logger.LogInformation($"Уведомления успешно отправлены через SignalR");
                    
                    // Дополнительно отправляем уведомление всем клиентам (на случай проблем с группами)
                    await duelHubContext.Clients.All.SendAsync("DuelCreated", newDuel.Id);
                    logger.LogInformation($"Отправлено уведомление всем клиентам через SignalR.All");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Ошибка при отправке уведомлений о создании дуэли {newDuel.Id} через SignalR");
                }
                
                // Отмечаем пользователей как обработанных
                processedUserIds.Add(user.Id);
                processedUserIds.Add(opponentId);
                
                logger.LogInformation($"Создана новая дуэль {newDuel.Id} между пользователями {user.Id} и {opponentId}");
            }
        }
    }
} 