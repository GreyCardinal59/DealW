
namespace DealW.Persistence.Entities;

public class QuestionEntity
{
    public int Id { get; set; }
    public int QuizId { get; set; }
    public string Text { get; set; } = string.Empty;
    public int CorrectAnswerId { get; set; }
    
    public virtual QuizEntity? Quiz { get; set; }
}