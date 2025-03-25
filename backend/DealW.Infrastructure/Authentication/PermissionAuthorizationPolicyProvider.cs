using DealW.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace DealW.Infrastructure.Authentication;

public class PermissionAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
    : DefaultAuthorizationPolicyProvider(options)
{
    public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        var policy = await base.GetPolicyAsync(policyName);

        if (policy is not null)
        {
            return policy;
        }
        
        var permissions = policyName.Split(',')
            .Select(p => Enum.TryParse<Permission>(p.Trim(), out var permission) ? permission : (Permission?)null)
            .Where(p => p.HasValue)
            .Select(p => p.Value)
            .ToArray();
        
        // if (policy == null)
        // {
        //     if (permissions.Length != 0)
        //     {
        //         policy = new AuthorizationPolicyBuilder()
        //             .AddRequirements(new PermissionRequirement(permissions))
        //             .Build();
        //         
        //         options.Value.AddPolicy(policyName, policy);
        //     }
        // }
        
        return new AuthorizationPolicyBuilder()
            .AddRequirements(new PermissionRequirement(permissions))
            .Build();
    }
}