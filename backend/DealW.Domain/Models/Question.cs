// namespace DealW.Domain.Models;
//
// public class Question
// {
//     // public const int MAX_TEXT_LENGHT = 250;
//     
//     // private Question(Guid id, string text, IList<string> answers, string correctAnswer)
//     // {
//     //     Id = id;
//     //     Text = text;
//     //     Answers = answers;
//     //     CorrectAnswer = correctAnswer;
//     // }
//     public Guid Id { get; set; }
//     public string Text { get; set; }
//     public IList<string> Answers { get; set; }
//     public string CorrectAnswer { get; set; }
//     
//     // public static (Question question, string Error) Create(Guid id, string text, IList<string> answers, string correctAnswer)
//     // {
//     //     var error = string.Empty;
//     //
//     //     if (string.IsNullOrEmpty(text) || text.Length > MAX_TEXT_LENGHT)
//     //     {
//     //         error = "Title can't be empty or longer than 250 symbols";
//     //     }
//     //     var question = new Question(id, text, answers, correctAnswer);
//     //
//     //     return (question, error);
//     // }
// }