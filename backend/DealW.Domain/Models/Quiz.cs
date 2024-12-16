namespace DealW.Domain.Models;

public class Quiz
{
    public const int MAX_TITLE_LENGHT = 250;
    private Quiz(int id, string title, string difficulty)
    {
        Id = id;
        Title = title;
        Difficulty = difficulty;
    }
    
    public int Id { get; }
    public string Title { get; }
    public string Difficulty { get; }
    
    public virtual ICollection<Question> Questions { get; }

    public static (Quiz Quiz, string Error) Create(int id, string title, string difficulty)
    {
        var error = string.Empty;
    
        if (string.IsNullOrEmpty(title) || title.Length > MAX_TITLE_LENGHT)
        {
            error = "Title can't be empty or longer than 250 symbols";
        }
        var quiz = new Quiz(id, title, difficulty);
    
        return (quiz, error);
    }
}