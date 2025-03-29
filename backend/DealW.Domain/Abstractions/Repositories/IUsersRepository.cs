using DealW.Domain.Models;

namespace DealW.Domain.Abstractions;

public interface IUsersRepository
{
    Task Add(User user);
    Task<User> GetByEmail(string email);
    Task<HashSet<Enums.Permission>> GetUserPermissions(Guid userId);
}