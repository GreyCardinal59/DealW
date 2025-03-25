using DealW.Domain.Abstractions;
using DealW.Domain.Enums;

namespace DealW.Application.Services;

public class PermissionService(IUsersRepository usersRepository) : IPermissionService
{

    public Task<HashSet<Permission>> GetPermissionsAsync(Guid userId)
    {
        return usersRepository.GetUserPermissions(userId);
    }
}