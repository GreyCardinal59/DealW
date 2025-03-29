namespace DealW.Persistence.Entities;

public class AnswerEntity
{
    public int Id { get; set; }
    public int DuelQuestionId { get; set; }
    public Guid UserId { get; set; }
    public string UserAnswer { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    
    public DuelQuestionEntity DuelQuestion { get; set; } = null!;
    public UserEntity User { get; set; } = null!;
}