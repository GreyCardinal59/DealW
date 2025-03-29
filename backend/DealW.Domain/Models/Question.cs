namespace DealW.Domain.Models;

public class Question
{
    private Question(string text, string correctAnswer)
    {
        // Id = id;
        Text = text;
        CorrectAnswer = correctAnswer;
    }
    public int Id { get; set; }
    public string Text { get; set; }
    public string CorrectAnswer { get; set; }
    // public string TestCases { get; set; } // JSON
    
    // TODO: add QuestionType property for different question types, like MultipleChoice or TextInput (enum)
}