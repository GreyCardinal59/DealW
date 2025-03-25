namespace DealW.Persistence.Entities;

/// <summary>
/// Связующая таблица Роль/Разрешения 
/// </summary>
public class RolePermissionEntity
{
    public int RoleId { get; set; }
    public int PermissionId { get; set; }
}