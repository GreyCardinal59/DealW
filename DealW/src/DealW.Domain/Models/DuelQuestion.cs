namespace DealW.Domain.Models;

public class DuelQuestion
{
    public int Id { get; set; }
    
    public int DuelId { get; set; }
    
    public int QuestionId { get; set; }
    
    public Duel Duel { get; set; }
    
    public Question Question { get; set; }
        
    public int QuestionOrder { get; set; } // Порядок вопроса в дуэли
        
    public string? FirstUserAnswer { get; set; }
        
    public string? SecondUserAnswer { get; set; }
        
    public bool? IsFirstUserAnswerCorrect { get; set; }
        
    public bool? IsSecondUserAnswerCorrect { get; set; }
        
    public DateTime? AnswerTime { get; set; }
        
    // Флаги отправки ответов
    public bool FirstUserSubmitted { get; set; } = false;
        
    public bool SecondUserSubmitted { get; set; } = false;
}