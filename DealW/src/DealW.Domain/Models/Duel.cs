namespace DealW.Domain.Models;

public class Duel
{
    public int Id { get; set; }
    
    public int FirstUserId { get; set; }
    
    public int SecondUserId { get; set; }
    
    public User FirstUser { get; set; }
    
    public User SecondUser { get; set; }
        
    public DateTime StartTime { get; set; } = DateTime.UtcNow;
        
    public DateTime? EndTime { get; set; }
        
    public int? WinnerId { get; set; }
    
    public User Winner { get; set; }
        
    public bool IsDraw { get; set; } = false;
        
    public bool IsCompleted { get; set; } = false;
        
    public int FirstUserCorrectAnswers { get; set; } = 0;
        
    public int SecondUserCorrectAnswers { get; set; } = 0;
        
    public string Status { get; set; } = "waiting"; // waiting, active, completed, cancelled
        
    // Связь с вопросами через промежуточную таблицу
    public ICollection<DuelQuestion> DuelQuestions { get; set; }
}