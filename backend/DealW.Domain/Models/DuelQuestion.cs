namespace DealW.Domain.Models;

public class DuelQuestion
{
    private DuelQuestion(Duel duel, Question question)
    {
        Duel = duel;
        Question = question;
    }
    
    public int Id { get; private set; }
    public Duel Duel { get; private set; }
    public Question Question { get; private set; }
    // public int Order { get; private set; }
}