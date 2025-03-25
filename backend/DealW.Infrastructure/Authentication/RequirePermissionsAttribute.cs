using DealW.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DealW.Infrastructure.Authentication;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class RequirePermissionsAttribute : AuthorizeAttribute
{
    public RequirePermissionsAttribute(params Permission[] permissions)
        : base(policy: string.Join(',', permissions.Select(p => p.ToString())))
    {
        
    }
}