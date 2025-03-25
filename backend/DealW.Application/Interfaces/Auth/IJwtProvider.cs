using DealW.Domain.Models;

namespace DealW.Application.Interfaces.Auth;

public interface IJwtProvider
{
    string GenerateToken(User user);
}