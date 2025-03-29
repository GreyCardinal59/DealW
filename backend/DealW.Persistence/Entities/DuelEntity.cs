
namespace DealW.Persistence.Entities;

public class DuelEntity
{
    public int Id { get; set; }
    
    public Guid User1Id { get; set; }
    public Guid User2Id { get; set; }
    public DateTime StartTime { get; set; }
    public Guid? WinnerId { get; set; }
    
    public UserEntity Player1 { get; set; } = null!;
    public UserEntity Player2 { get; set; } = null!;
    public ICollection<DuelQuestionEntity> Questions { get; set; } = new List<DuelQuestionEntity>();
    public DuelStatus Status { get; set; } 
    
    public enum DuelStatus
    {
        Waiting, // Ожидание соперника
        InProgress, // В процессе
        Finished // Завершена
    }
}