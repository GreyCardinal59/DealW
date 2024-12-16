using DealW.Domain.Models;

namespace DealW.Persistence.Entities;

public class QuizEntity
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;
    public virtual IEnumerable<QuestionEntity>? Questions { get; set; }
}