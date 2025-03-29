namespace DealW.Domain.Models;

public class Answer
{
    private Answer(DuelQuestion duelQuestion, Guid userId, string userAnswer, bool isCorrect)
    {
        DuelQuestion = duelQuestion;
        UserId = userId;
        UserAnswer = userAnswer;
        IsCorrect = isCorrect;
    }
    
    public int Id { get; set; }
    public DuelQuestion DuelQuestion { get; set; }
    public Guid UserId { get; set; }
    public string UserAnswer { get; set; }
    public bool IsCorrect { get; set; }
    
    // Для возможных вариантов ответа
    // public int? AnswerOptionId { get; set; }
    // public AnswerOption? AnswerOption { get; set; }
}