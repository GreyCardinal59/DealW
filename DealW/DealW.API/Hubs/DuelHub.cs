using System.Collections.Concurrent;
using DealW.Application.Services;
using Microsoft.AspNetCore.SignalR;

namespace DealW.API.Hubs
{
    public class DuelHub(
        DuelService duelService,
        UserService userService,
        ILogger<DuelHub> logger,
        IServiceScopeFactory serviceScopeFactory,
        IRedisService redisService)
        : Hub
    {
        private static readonly Dictionary<string, int> ConnectionToUserId = 
            new Dictionary<string, int>();
        
        private static readonly ConcurrentDictionary<int, CancellationTokenSource> SearchTasks = 
            new ConcurrentDictionary<int, CancellationTokenSource>();
        
        // Максимальное время поиска оппонента (в секундах)
        private const int MAX_SEARCH_TIME = 60;
        // Интервал между попытками найти оппонента (в миллисекундах)
        private const int SEARCH_INTERVAL = 1000;

        // Обработчик события нахождения совпадения
        private async void OnMatchFound(int firstUserId, int secondUserId)
        {
            try
            {
                logger.LogInformation($"Обработка совпадения между пользователями {firstUserId} и {secondUserId}");
                
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    // Получаем сервис дуэлей из нового scope
                    var scopedDuelService = scope.ServiceProvider.GetRequiredService<DuelService>();
                    
                    // Создаем дуэль между пользователями
                    var duel = await scopedDuelService.CreateDuelAsync(firstUserId, secondUserId);
                    
                    if (duel != null)
                    {
                        logger.LogInformation($"Дуэль {duel.Id} создана между пользователями {firstUserId} и {secondUserId}");
                        
                        // Отменяем задачи поиска для обоих пользователей
                        if (SearchTasks.TryRemove(firstUserId, out var firstCts))
                        {
                            await firstCts.CancelAsync();
                        }
                        
                        if (SearchTasks.TryRemove(secondUserId, out var secondCts))
                        {
                            await secondCts.CancelAsync();
                        }
                        
                        // Уведомляем пользователей о начале дуэли
                        await Clients.Group($"user_{firstUserId}").SendAsync("DuelCreated", duel.Id);
                        await Clients.Group($"user_{secondUserId}").SendAsync("DuelCreated", duel.Id);
                    }
                    else
                    {
                        logger.LogError($"Не удалось создать дуэль между пользователями {firstUserId} и {secondUserId}");
                        // Отправляем ошибку участникам
                        await Clients.Group($"user_{firstUserId}").SendAsync("ErrorOccurred", "Не удалось создать дуэль. Пожалуйста, попробуйте еще раз.");
                        await Clients.Group($"user_{secondUserId}").SendAsync("ErrorOccurred", "Не удалось создать дуэль. Пожалуйста, попробуйте еще раз.");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Ошибка при обработке совпадения между пользователями {firstUserId} и {secondUserId}");
            }
        }

        /// <summary>
        /// Регистрирует соединение пользователя
        /// </summary>
        public async Task RegisterConnection(int userId)
        {
            try 
            {
                // Проверяем существование пользователя
                var user = await userService.GetUserByIdAsync(userId);
                if (user == null)
                {
                    logger.LogWarning($"Попытка регистрации соединения для несуществующего пользователя {userId}");
                    await Clients.Caller.SendAsync("ErrorOccurred", $"Пользователь с ID {userId} не найден в базе данных");
                    return;
                }
                
                // Удаляем пользователя из любых предыдущих групп с тем же ID
                string groupName = $"user_{userId}";
                var connectionIds = ConnectionToUserId.Where(kv => kv.Value == userId).Select(kv => kv.Key).ToList();
                
                foreach (var existingConnectionId in connectionIds)
                {
                    if (existingConnectionId != Context.ConnectionId)
                    {
                        logger.LogInformation($"Удаление предыдущего соединения {existingConnectionId} пользователя {userId} из группы {groupName}");
                        await Groups.RemoveFromGroupAsync(existingConnectionId, groupName);
                        ConnectionToUserId.Remove(existingConnectionId);
                    }
                }
                
                // Добавляем соединение в словарь пользователей
                ConnectionToUserId[Context.ConnectionId] = userId;
                
                // Добавляем соединение в группу пользователя
                await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
                
                logger.LogInformation($"Пользователь {userId} зарегистрировал соединение {Context.ConnectionId} и добавлен в группу {groupName}");
                
                // Отправляем подтверждение успешной регистрации
                await Clients.Caller.SendAsync("ConnectionRegistered", userId);
                
                // Проверяем, есть ли у пользователя активная дуэль
                var activeDuel = await duelService.GetUserActiveDuelAsync(userId);
                if (activeDuel != null)
                {
                    logger.LogInformation($"У пользователя {userId} есть активная дуэль {activeDuel.Id}, отправляем уведомление");
                    await Clients.Caller.SendAsync("DuelCreated", activeDuel.Id);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Ошибка при регистрации соединения пользователя {userId}");
                await Clients.Caller.SendAsync("ErrorOccurred", $"Ошибка при регистрации соединения: {ex.Message}");
            }
        }

        /// <summary>
        /// Начинает поиск оппонента для пользователя
        /// </summary>
        public async Task StartSearchingOpponent(int userId)
        {
            try
            {
                logger.LogInformation($"Пользователь {userId} начал поиск оппонента");
                
                // Проверяем, существует ли пользователь
                var user = await userService.GetUserByIdAsync(userId);
                if (user == null)
                {
                    await Clients.Caller.SendAsync("ErrorOccurred", "Пользователь не найден");
                    return;
                }
                
                // Проверяем, не находится ли пользователь уже в активной дуэли
                var activeDuel = await duelService.GetUserActiveDuelAsync(userId);
                
                if (activeDuel != null)
                {
                    await Clients.Caller.SendAsync("ErrorOccurred", "Вы уже участвуете в активной дуэли");
                    return;
                }
                
                // Обновляем статус поиска в базе данных
                await userService.UpdateSearchStatusAsync(userId, true);
                
                // Добавляем пользователя в очередь Redis
                await redisService.AddUserToMatchmakingQueueAsync(userId, user.Rating);
                
                // Оповещаем клиента о начале поиска
                // Отправляем userId для обратной совместимости
                await Clients.Caller.SendAsync("SearchStarted", userId);
                
                // Сохраняем соответствие между соединением и ID пользователя
                ConnectionToUserId[Context.ConnectionId] = userId;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Ошибка при начале поиска оппонента для пользователя {userId}");
                await Clients.Caller.SendAsync("ErrorOccurred", "Внутренняя ошибка сервера");
            }
        }
        
        /// <summary>
        /// Отслеживает таймаут поиска и уведомляет пользователя, если оппонент не найден
        /// </summary>
        private async Task MonitorSearchTimeout(int userId, CancellationToken cancellationToken)
        {
            try
            {
                // Ждем срабатывания таймаута или отмены
                try
                {
                    await Task.Delay(MAX_SEARCH_TIME * 1000, cancellationToken);
                }
                catch (TaskCanceledException)
                {
                    // Поиск был отменен (например, найден оппонент)
                    logger.LogInformation($"Поиск для пользователя {userId} был отменен");
                    return;
                }
                
                // Если мы здесь, значит истекло время поиска
                logger.LogInformation($"Истекло время поиска для пользователя {userId}");
                
                // Отправляем уведомление пользователю
                await Clients.Group($"user_{userId}").SendAsync("OpponentNotFound", cancellationToken);
                
                // Очищаем токен поиска
                SearchTasks.TryRemove(userId, out _);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Ошибка при мониторинге времени поиска для пользователя {userId}");
            }
        }

        /// <summary>
        /// Отменяет поиск оппонента
        /// </summary>
        public async Task CancelSearchingOpponent(int userId)
        {
            try
            {
                logger.LogInformation($"Пользователь {userId} отменил поиск оппонента");
                
                // Удаляем пользователя из очереди Redis
                await redisService.RemoveUserFromMatchmakingQueueAsync(userId);
                
                // Обновляем статус поиска в БД
                await userService.UpdateSearchStatusAsync(userId, false);
                
                // Оповещаем клиента об отмене поиска
                // Отправляем userId для обратной совместимости
                await Clients.Caller.SendAsync("SearchCancelled", userId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Ошибка при отмене поиска оппонента для пользователя {userId}");
                await Clients.Caller.SendAsync("ErrorOccurred", "Внутренняя ошибка сервера");
            }
        }

        /// <summary>
        /// Отправляет ответ пользователя на вопрос
        /// </summary>
        public async Task SubmitAnswer(int duelId, int questionOrder, int userId, string answer)
        {
            try
            {
                logger.LogInformation($"Получен ответ от пользователя {userId} на вопрос {questionOrder} дуэли {duelId}: {answer}");
                
                // Проверяем, что пользователь участвует в дуэли
                var duel = await redisService.GetActiveDuelAsync(duelId);
                if (duel == null)
                {
                    // Пробуем получить из базы данных
                    duel = await duelService.GetDuelByIdAsync(duelId);
                    if (duel == null)
                    {
                        await Clients.Caller.SendAsync("ErrorOccurred", "Дуэль не найдена");
                        return;
                    }
                }
                
                if (duel.FirstUserId != userId && duel.SecondUserId != userId)
                {
                    await Clients.Caller.SendAsync("ErrorOccurred", "Вы не участвуете в этой дуэли");
                    return;
                }
                
                // Проверяем, что текущий вопрос активен
                var currentQuestion = await redisService.GetActiveQuestionAsync(duelId);
                if (currentQuestion != questionOrder)
                {
                    await Clients.Caller.SendAsync("ErrorOccurred", "Этот вопрос не является активным");
                    return;
                }
                
                // Проверяем, что время на ответ еще не истекло
                var remainingTime = await redisService.GetRemainingTimeAsync(duelId, questionOrder);
                if (remainingTime <= 0)
                {
                    await Clients.Caller.SendAsync("ErrorOccurred", "Время на ответ истекло");
                    return;
                }
                
                // Сохраняем ответ пользователя напрямую в Redis
                await redisService.SaveUserAnswerAsync(duelId, questionOrder, userId, answer);
                
                // Уведомляем пользователя о принятии ответа
                await Clients.Caller.SendAsync("AnswerSubmitted", duelId, questionOrder);
                
                // Проверяем, ответили ли оба пользователя, используя Redis
                var answers = await redisService.GetQuestionAnswersInfoAsync(
                    duelId, 
                    questionOrder, 
                    duel.FirstUserId, 
                    duel.SecondUserId
                );
                
                if (!(!answers.firstUserSubmitted || !answers.secondUserSubmitted))
                {
                    // Получаем HubContext для передачи в ProcessQuestionResultsAsync
                    var hubContext = Context.GetHttpContext()?.RequestServices.GetService(typeof(IHubContext<DuelHub>)) as IHubContext<DuelHub>;
                    
                    // Оба участника ответили, переходим к результатам
                    await Clients.Group($"user_{duel.FirstUserId}").SendAsync("BothAnswersSubmitted", duelId, questionOrder);
                    await Clients.Group($"user_{duel.SecondUserId}").SendAsync("BothAnswersSubmitted", duelId, questionOrder);
                    
                    // Обрабатываем результаты немедленно, не дожидаясь истечения таймера
                    await ProcessQuestionResultsAsync(duelId, questionOrder, hubContext);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Ошибка при отправке ответа на вопрос {questionOrder} дуэли {duelId}");
                await Clients.Caller.SendAsync("ErrorOccurred", "Внутренняя ошибка сервера");
            }
        }

        /// <summary>
        /// Обрабатывает отключение клиента
        /// </summary>
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (ConnectionToUserId.TryGetValue(Context.ConnectionId, out int userId))
            {
                // Отменяем поиск оппонента, если пользователь был в процессе поиска
                if (SearchTasks.TryRemove(userId, out var cts))
                {
                    await cts.CancelAsync();
                }
                
                // Удаляем соединение из словаря
                ConnectionToUserId.Remove(Context.ConnectionId);
            }
            
            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Проверяет текущее состояние поиска оппонента
        /// </summary>
        public async Task CheckSearchStatus(int userId)
        {
            try
            {
                // Проверяем, существует ли пользователь
                var user = await userService.GetUserByIdAsync(userId);
                if (user == null)
                {
                    await Clients.Caller.SendAsync("ErrorOccurred", $"Пользователь с ID {userId} не найден в базе данных");
                    return;
                }
            
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("ErrorOccurred", ex.Message);
            }
        }
        
        /// <summary>
        /// Получает оставшееся время поиска
        /// </summary>
        private int GetTimeRemaining(int userId)
        {
            if (SearchTasks.TryGetValue(userId, out var cts))
            {
                // Проверяем, не отменен ли уже токен
                if (cts.IsCancellationRequested)
                    return 0;
                
                // Проверяем, можем ли мы получить токен с таймаутом
                if (cts.Token.CanBeCanceled && cts.Token is CancellationToken token && 
                    token.IsCancellationRequested == false)
                {
                    // Максимальное время поиска в секундах
                    return MAX_SEARCH_TIME;
                }
                
                // Если не можем определить точно, возвращаем приблизительное время
                return MAX_SEARCH_TIME / 2;
            }
            
            return 0;
        }

        /// <summary>
        /// Начинает дуэль с первого вопроса
        /// </summary>
        public async Task StartDuel(int duelId)
        {
            try
            {
                logger.LogInformation($"Запрос на начало дуэли {duelId}");
                
                // Получаем дуэль из Redis или базы данных
                var duel = await redisService.GetActiveDuelAsync(duelId);
                if (duel == null)
                {
                    // Если дуэль не найдена в Redis, пробуем найти в базе данных
                    duel = await duelService.GetDuelByIdAsync(duelId);
                    if (duel == null)
                    {
                        await Clients.Caller.SendAsync("ErrorOccurred", "Дуэль не найдена");
                        return;
                    }
                    
                    // Сохраняем дуэль в Redis для быстрого доступа
                    await redisService.SaveActiveDuelAsync(duel);
                }
                
                // Проверяем, не запущена ли уже дуэль (проверяем наличие активного вопроса)
                var currentQuestion = await redisService.GetActiveQuestionAsync(duelId);
                if (currentQuestion >= 0)
                {
                    // Дуэль уже запущена, просто отправляем текущее состояние клиенту
                    logger.LogInformation($"Дуэль {duelId} уже запущена с активным вопросом {currentQuestion}, пропускаем повторную инициализацию");
                    await Clients.Caller.SendAsync("DuelStarted", duelId, currentQuestion);
                    return;
                }
                
                // Используем семафор для синхронизации запуска дуэли
                using (var semaphore = new SemaphoreSlim(1, 1))
                {
                    await semaphore.WaitAsync();
                    try
                    {
                        // Повторно проверяем, не запущена ли уже дуэль после ожидания семафора
                        currentQuestion = await redisService.GetActiveQuestionAsync(duelId);
                        if (currentQuestion >= 0)
                        {
                            logger.LogInformation($"Дуэль {duelId} была запущена другим клиентом, пропускаем повторную инициализацию");
                            await Clients.Caller.SendAsync("DuelStarted", duelId, currentQuestion);
                            return;
                        }
                        
                        // Устанавливаем первый вопрос как активный
                        await redisService.SetActiveQuestionAsync(duelId, 0);
                        
                        // Запускаем таймер для первого вопроса
                        await redisService.StartQuestionTimerAsync(duelId, 0);
                        
                        // Получаем HubContext для передачи в MonitorQuestionTimeAsync
                        var hubContext = Context.GetHttpContext()?.RequestServices.GetService(typeof(IHubContext<DuelHub>)) as IHubContext<DuelHub>;
                        
                        // Оповещаем обоих участников о начале дуэли и первом вопросе
                        await Clients.Group($"user_{duel.FirstUserId}").SendAsync("DuelStarted", duelId, 0);
                        await Clients.Group($"user_{duel.SecondUserId}").SendAsync("DuelStarted", duelId, 0);
                        
                        // Запускаем задачу для контроля времени на ответ
                        _ = MonitorQuestionTimeAsync(duelId, 0, hubContext);
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Ошибка при начале дуэли {duelId}");
                await Clients.Caller.SendAsync("ErrorOccurred", "Внутренняя ошибка сервера");
            }
        }
        
        /// <summary>
        /// Отслеживает время на ответ для текущего вопроса
        /// </summary>
        private async Task MonitorQuestionTimeAsync(int duelId, int questionOrder, IHubContext<DuelHub> hubContext = null)
        {
            try
            {
                // Если hubContext не был передан, не пытаемся получить его из Context
                // так как Context может быть уже недоступен
                
                // Ждем 15 секунд (время на ответ)
                await Task.Delay(15000);
                
                // Получаем дуэль
                var duel = await redisService.GetActiveDuelAsync(duelId);
                if (duel == null)
                {
                    logger.LogWarning($"Не удалось найти дуэль {duelId} для проверки времени");
                    return;
                }
                
                // Проверяем, что текущий вопрос всё ещё активен
                var currentQuestion = await redisService.GetActiveQuestionAsync(duelId);
                if (currentQuestion != questionOrder)
                {
                    logger.LogInformation($"Вопрос {questionOrder} больше не активен для дуэли {duelId}");
                    return;
                }
                
                // Получаем состояние ответов из Redis
                var answers = await redisService.GetQuestionAnswersInfoAsync(
                    duelId, 
                    questionOrder, 
                    duel.FirstUserId, 
                    duel.SecondUserId
                );
                
                // Автоматически отправляем пустые ответы для участников, которые не успели ответить
                // Используем транзакцию для атомарной проверки и установки ответов
                using (var semaphore = new SemaphoreSlim(1, 1))
                {
                    await semaphore.WaitAsync();
                    try
                    {
                        // Повторно проверяем состояние ответов, так как они могли измениться
                        var updatedAnswers = await redisService.GetQuestionAnswersInfoAsync(
                            duelId, 
                            questionOrder, 
                            duel.FirstUserId, 
                            duel.SecondUserId
                        );
                        
                        if (!updatedAnswers.firstUserSubmitted)
                        {
                            await redisService.SaveUserAnswerAsync(duelId, questionOrder, duel.FirstUserId, "");
                            logger.LogInformation($"Время истекло: автоматически отправлен пустой ответ для пользователя {duel.FirstUserId}");
                        }
                        
                        if (!updatedAnswers.secondUserSubmitted)
                        {
                            await redisService.SaveUserAnswerAsync(duelId, questionOrder, duel.SecondUserId, "");
                            logger.LogInformation($"Время истекло: автоматически отправлен пустой ответ для пользователя {duel.SecondUserId}");
                        }
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }
                
                // Оповещаем пользователей о завершении времени ответа через hubContext вместо this.Clients
                if (hubContext != null)
                {
                    await hubContext.Clients.Group($"user_{duel.FirstUserId}").SendAsync("QuestionTimeEnded", duelId, questionOrder);
                    await hubContext.Clients.Group($"user_{duel.SecondUserId}").SendAsync("QuestionTimeEnded", duelId, questionOrder);
                    
                    // Проверяем результаты ответов - создаем и запускаем новый метод через DI
                    await ProcessQuestionResultsAsync(duelId, questionOrder, hubContext);
                }
                else
                {
                    logger.LogWarning($"Не удалось получить HubContext для дуэли {duelId} при мониторинге времени вопроса {questionOrder}");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Ошибка при мониторинге времени вопроса {questionOrder} для дуэли {duelId}");
            }
        }
        
        /// <summary>
        /// Обрабатывает результаты ответов на вопрос
        /// </summary>
        private async Task ProcessQuestionResultsAsync(int duelId, int questionOrder, IHubContext<DuelHub> hubContext = null)
        {
            try
            {
                // Если hubContext не передан, просто логируем это, но не пытаемся получить его из Context
                if (hubContext == null)
                {
                    logger.LogWarning($"HubContext не был передан для обработки результатов вопроса {questionOrder} дуэли {duelId}");
                }
                
                // Получаем дуэль
                var duel = await redisService.GetActiveDuelAsync(duelId);
                if (duel == null)
                {
                    logger.LogWarning($"Не удалось найти дуэль {duelId} для обработки результатов");
                    return;
                }
                
                // Получаем ответы пользователей из Redis
                var answers = await redisService.GetQuestionAnswersInfoAsync(
                    duelId, 
                    questionOrder, 
                    duel.FirstUserId, 
                    duel.SecondUserId
                );
                
                // Пытаемся получить вопрос из Redis
                var question = await redisService.GetDuelQuestionAsync(duelId, questionOrder);
                
                if (question == null)
                {
                    logger.LogWarning($"Не удалось получить данные вопроса {questionOrder} для дуэли {duelId}");
                    return;
                }
                
                // Проверяем правильность ответов
                string correctAnswer = question.CorrectAnswer?.Trim().ToLowerInvariant() ?? string.Empty;
                string firstUserAnswer = answers.firstUserAnswer?.Trim().ToLowerInvariant() ?? string.Empty;
                string secondUserAnswer = answers.secondUserAnswer?.Trim().ToLowerInvariant() ?? string.Empty;
                
                bool firstUserCorrect = firstUserAnswer.Equals(correctAnswer);
                bool secondUserCorrect = secondUserAnswer.Equals(correctAnswer);
                
                // Отправляем результаты пользователям
                var results = new
                {
                    QuestionId = question.Id,
                    QuestionText = question.Text,
                    CorrectAnswer = question.CorrectAnswer,
                    FirstUserAnswer = answers.firstUserAnswer,
                    SecondUserAnswer = answers.secondUserAnswer,
                    IsFirstUserCorrect = firstUserCorrect,
                    IsSecondUserCorrect = secondUserCorrect
                };
                
                if (hubContext != null)
                {
                    await hubContext.Clients.Group($"user_{duel.FirstUserId}").SendAsync("QuestionResults", duelId, questionOrder, results);
                    await hubContext.Clients.Group($"user_{duel.SecondUserId}").SendAsync("QuestionResults", duelId, questionOrder, results);
                }
                
                // Проверяем, есть ли следующий вопрос
                if (questionOrder < 4) // У нас 5 вопросов, индексы 0-4
                {
                    // Переходим к следующему вопросу
                    var nextQuestion = questionOrder + 1;
                    
                    // Даем небольшую паузу перед следующим вопросом
                    await Task.Delay(3000);
                    
                    // Создаем транзакцию с использованием семафора, чтобы избежать дублирования логов
                    using (var semaphore = new SemaphoreSlim(1, 1))
                    {
                        await semaphore.WaitAsync();
                        try
                        {
                            // Устанавливаем следующий вопрос как активный
                            await redisService.SetActiveQuestionAsync(duelId, nextQuestion);
                            
                            // Запускаем таймер для следующего вопроса
                            await redisService.StartQuestionTimerAsync(duelId, nextQuestion);
                            
                            // Оповещаем участников о следующем вопросе
                            if (hubContext != null)
                            {
                                await hubContext.Clients.Group($"user_{duel.FirstUserId}").SendAsync("NextQuestion", duelId, nextQuestion);
                                await hubContext.Clients.Group($"user_{duel.SecondUserId}").SendAsync("NextQuestion", duelId, nextQuestion);
                            }
                        }
                        finally
                        {
                            semaphore.Release();
                        }
                    }
                    
                    // Запускаем мониторинг времени для нового вопроса
                    _ = MonitorQuestionTimeAsync(duelId, nextQuestion, hubContext);
                }
                else
                {
                    // Это был последний вопрос, завершаем дуэль
                    await FinalizeDuelAsync(duelId, hubContext);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Ошибка при обработке результатов вопроса {questionOrder} для дуэли {duelId}");
            }
        }
        
        /// <summary>
        /// Завершает дуэль и определяет победителя
        /// </summary>
        private async Task FinalizeDuelAsync(int duelId, IHubContext<DuelHub> hubContext = null)
        {
            try
            {
                // Если hubContext не передан, просто логируем это, но не пытаемся получить его из Context
                if (hubContext == null)
                {
                    logger.LogWarning($"HubContext не был передан для финализации дуэли {duelId}");
                }
                
                logger.LogInformation($"Завершение дуэли {duelId}");
                
                try
                {
                    using (var scope = serviceScopeFactory.CreateScope())
                    {
                        // Получаем сервисы из нового scope
                        var scopedDuelService = scope.ServiceProvider.GetRequiredService<DuelService>();
                        var scopedRedisService = scope.ServiceProvider.GetRequiredService<IRedisService>();
                        
                        // Получаем дуэль и подводим итоги
                        await scopedDuelService.CompleteDuelAsync(duelId);
                        
                        // Получаем обновленные данные дуэли с использованием нового запроса
                        var duel = await scopedDuelService.GetDuelByIdAsync(duelId);
                        if (duel == null)
                        {
                            logger.LogWarning($"Не удалось получить дуэль {duelId} для финализации");
                            return;
                        }
                        
                        // Отправляем финальные результаты обоим пользователям
                        var finalResults = new
                        {
                            DuelId = duel.Id,
                            duel.FirstUserId,
                            duel.SecondUserId,
                            FirstUserScore = duel.FirstUserCorrectAnswers,
                            SecondUserScore = duel.SecondUserCorrectAnswers,
                            duel.WinnerId,
                            duel.IsDraw
                        };
                        
                        if (hubContext != null)
                        {
                            await hubContext.Clients.Group($"user_{duel.FirstUserId}").SendAsync("DuelCompleted", finalResults);
                            await hubContext.Clients.Group($"user_{duel.SecondUserId}").SendAsync("DuelCompleted", finalResults);
                        }
                        
                        // Удаляем дуэль из Redis
                        await scopedRedisService.RemoveActiveDuelAsync(duelId);
                    }
                }
                catch (ObjectDisposedException ex)
                {
                    logger.LogError(ex, $"Ошибка доступа к освобожденному контексту при финализации дуэли {duelId}");
                    // В случае ошибки с контекстом, пробуем удалить дуэль из Redis
                    await redisService.RemoveActiveDuelAsync(duelId);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Ошибка при финализации дуэли {duelId}");
            }
        }
    }
} 