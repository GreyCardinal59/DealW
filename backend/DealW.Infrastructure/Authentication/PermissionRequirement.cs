using DealW.Domain.Enums;
using Microsoft.AspNetCore.Authorization;

namespace DealW.Infrastructure.Authentication;

public class PermissionRequirement() : IAuthorizationRequirement
{
    public Permission[] Permissions { get; set; } = null!;

    public PermissionRequirement(params Permission[] permissions) : this()
    {
        Permissions = permissions;
    }
    
}