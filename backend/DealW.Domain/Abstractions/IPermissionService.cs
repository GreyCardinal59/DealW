using DealW.Domain.Enums;

namespace DealW.Domain.Abstractions;

public interface IPermissionService
{
    Task<HashSet<Permission>> GetPermissionsAsync(Guid userId);
}