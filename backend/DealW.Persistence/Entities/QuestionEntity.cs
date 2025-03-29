
namespace DealW.Persistence.Entities;

public class QuestionEntity
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public string CorrectAnswer { get; set; } = string.Empty;

    public ICollection<DuelQuestionEntity> DuelQuestions { get; set; } = [];
}