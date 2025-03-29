using DealW.Domain.Enums;

namespace DealW.Domain.Models;

public class Duel
{
    private Duel(Guid user1Id, Guid user2Id)
    {
        User1Id = user1Id;
        User2Id = user2Id;
        Status = DuelStatus.Started;
        StartTime = DateTime.UtcNow;
    }
    
    public int Id { get; set; }
    public Guid User1Id { get; set; }
    public Guid User2Id { get; set; }
    public DuelStatus Status { get; set; } 
    public DateTime StartTime { get; set; }
    public Guid? WinnerId { get; set; }

    public void Start()
    {
        Status = DuelStatus.Started;
        StartTime = DateTime.UtcNow;
    }

    public void End(Guid winnerId)
    {
        WinnerId = winnerId;
        Status = DuelStatus.Completed;
    }
}