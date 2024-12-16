using DealW.Domain.Models;

namespace DealW.Domain.Abstractions;

public interface IQuestionsService
{
    // Task<List<Question>> GetAllQuestions();
    Task<List<Question>> GetByQuizId(int quizId);
    Task<int> CreateQuestion(Question question);
    Task<int> UpdateQuestion(int id, string text, int correctAnswer);
    Task<int> DeleteQuestion(int id);
}