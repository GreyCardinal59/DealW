namespace DealW.Domain.Models;

public class Quiz
{
    public const int MAX_TITLE_LENGHT = 250;
    private Quiz(Guid id, string title, IList<string> questions)
    {
        Id = id;
        Title = title;
        Questions = questions;
    }
    public Guid Id { get; }
    public string Title { get; }
    public IList<string> Questions { get; }

    public static (Quiz Quiz, string Error) Create(Guid id, string title, IList<string> questions)
    {
        var error = string.Empty;
    
        if (string.IsNullOrEmpty(title) || title.Length > MAX_TITLE_LENGHT)
        {
            error = "Title can't be empty or longer than 250 symbols";
        }
        var quiz = new Quiz(id, title, questions);
    
        return (quiz, error);
    }
}