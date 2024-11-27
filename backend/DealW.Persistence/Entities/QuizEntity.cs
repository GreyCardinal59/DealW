namespace DealW.Persistence.Entities;

public class QuizEntity
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public IList<string> Questions { get; set; }
}