using DealW.Domain.Enums;
using DealW.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DealW.Persistence.Configurations;

/// <summary>
/// Присвоение разрешений для каждой из ролей
/// </summary>
public partial class RolePermissionConfiguration(AuthorizationOptions authorizationOptions)
    : IEntityTypeConfiguration<RolePermissionEntity>
{

    public void Configure(EntityTypeBuilder<RolePermissionEntity> builder)
    {
        builder.HasKey(r => new { r.RoleId, r.PermissionId });

        builder.HasData(ParseRolePermissions());
    }
    
    /// <summary>
    /// Метод преобразует конфигурацию ролей и разрешений в массив сущностей RolePermissionEntity,
    /// связывая каждую роль с соответствующими разрешениями для настройки таблицы БД
    /// </summary>
    /// <returns></returns>
    private RolePermissionEntity[] ParseRolePermissions()
    {
        return authorizationOptions.RolePermissions
            .SelectMany(rp => rp.Permissions
                .Select(p => new RolePermissionEntity
                {
                    RoleId = (int)Enum.Parse<Role>(rp.Role),
                    PermissionId = (int)Enum.Parse<Permission>(p)
                }))
            .ToArray();
    }
}