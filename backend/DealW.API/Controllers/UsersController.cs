using DealW.Application.Services;
using DealW.Contracts.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DealW.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    [HttpPost("register")]
    public async Task<IResult> Register([FromBody] RegisterUserRequest request,UsersService usersService)
    {
        await usersService.Register(request.UserName, request.Email, request.Password);
        return Results.Ok();
    }
    
    [HttpPost("login")]
    public async Task<IResult> Login([FromBody] LoginUserRequest request, UsersService usersService)
    {
        var token = await usersService.Login(request.Email, request.Password);
        
        HttpContext.Response.Cookies.Append("tasty-cookies", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict
        });
        
        return Results.Ok(token);
    }
}