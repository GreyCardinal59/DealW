namespace DealW.Domain.Models;

public class Question
{
    private Question(int id, int quizId, string text, int correctAnswerId)
    {
        Id = id;
        QuizId = quizId;
        Text = text;
        CorrectAnswerId = correctAnswerId;
    }
    public int Id { get; }
    public int QuizId { get; }
    public string Text { get; }
    public int CorrectAnswerId { get; }
    
    public virtual Quiz Quiz { get; }
    // public virtual ICollection<Answer> Answers { get; }
    
    public static (Question Question, string Error) Create(int id, int quizId, string text, int correctAnswerId)
    {
        var error = string.Empty;
    
        if (string.IsNullOrEmpty(text))
        {
            error = "Question text can't be empty.";
        }
        
        var question = new Question(0, quizId, text, correctAnswerId);
    
        return (question, error);
    }
}