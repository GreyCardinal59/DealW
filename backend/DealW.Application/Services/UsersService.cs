using DealW.Application.Interfaces.Auth;
using DealW.Domain.Abstractions;
using DealW.Domain.Models;

namespace DealW.Application.Services;

public class UsersService(IUsersRepository usersRepository, IPasswordHasher passwordHasher, IJwtProvider jwtProvider) 
    : IUserService
{
    public async Task Register(string userName, string email, string password)
    {
        var hashedPassword = passwordHasher.Generate(password);
        
        var user = User.Create(Guid.NewGuid(), userName, hashedPassword, email);
        
        await usersRepository.Add(user);
    }

    public async Task<string> Login(string email, string password)
    {
        var user = await usersRepository.GetByEmail(email);

        var result = passwordHasher.Verify(password, user.PasswordHash);

        if (result == false)
        {
            throw new Exception("Failed to login");
        }
        
        var token = jwtProvider.GenerateToken(user);
        
        return token;
    }
}