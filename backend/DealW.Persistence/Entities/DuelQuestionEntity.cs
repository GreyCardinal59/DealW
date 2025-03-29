namespace DealW.Persistence.Entities;

public class DuelQuestionEntity
{
    public int Id { get; set; }
    public int DuelId { get; set; }
    public int QuestionId { get; set; }
    
    
    public DuelEntity Duel { get; set; } = null!;
    public QuestionEntity Question { get; set; } = null!;
    public ICollection<AnswerEntity> Answers { get; set; } = new List<AnswerEntity>();
}