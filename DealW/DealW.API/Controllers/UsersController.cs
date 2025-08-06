using System.Security.Claims;
using DealW.Application.Extensions;
using DealW.Application.Models.Pagination;
using DealW.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DealW.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(
        UserService userService,
        AuthService authService) : ControllerBase
    {
        /// <summary>
        /// Регистрирует нового пользователя
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var user = await userService.RegisterUserAsync(model.Username, model.Email, model.Password);
                return Ok(new { user.Id, user.Username, user.Email });
            }
            catch (Exception ex)
            {
                return BadRequest(new { ex.Message });
            }
        }

        /// <summary>
        /// Аутентифицирует пользователя
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await userService.AuthenticateUserAsync(model.UsernameOrEmail, model.Password);

            // Генерируем JWT токен для пользователя
            string token = authService.GenerateJwtToken(user);

            // Возвращаем информацию о пользователе с токеном
            return Ok(new {
                user.Id,
                user.Username,
                user.Email,
                Token = token 
            });
        }

        /// <summary>
        /// Получает профиль текущего пользователя
        /// </summary>
        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = await userService.GetUserByIdAsync(userId);
            
            if (user == null)
                return NotFound(new { Message = "Пользователь не найден" });

            return Ok(new 
            {
                user.Id,
                user.Username,
                user.Email,
                user.Rating,
                user.TotalWins,
                user.TotalLosses
            });
        }

        /// <summary>
        /// Получает топ пользователей по рейтингу
        /// </summary>
        [HttpGet("leaderboard")]
        public async Task<IActionResult> GetLeaderboard([FromQuery] PaginationParams paginationParams, [FromQuery] SortParams? sortParams = null)
        {
            var usersQuery = await userService.GetAllUsersQueryForLeaderboardAsync();
            
            var logger = HttpContext.RequestServices.GetRequiredService<ILogger<UsersController>>();
            var paginatedUsers = usersQuery.AsPaginated(paginationParams, sortParams, logger);
            
            var result = paginatedUsers.Items.Select(u => new 
            {
                u.Id,
                u.Username,
                u.Rating,
                u.TotalWins,
                u.TotalLosses
            });
            
            return Ok(new
            {
                items = result,
                paginationParams = paginatedUsers.PaginationParams,
                totalPages = paginatedUsers.TotalPages,
                hasNextPage = paginatedUsers.HasNextPage,
                hasPreveiwPage = paginatedUsers.HasPreviewPage
            });
        }
        
        /// <summary>
        /// Проверяет существование пользователя по ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await userService.GetUserByIdAsync(id);
            
            if (user == null)
                return NotFound(new { Message = $"Пользователь с ID {id} не найден" });

            return Ok(new 
            {
                user.Id, user.Username
            });
        }
    }

    public class RegisterModel
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginModel
    {
        public string UsernameOrEmail { get; set; }
        public string Password { get; set; }
    }
} 