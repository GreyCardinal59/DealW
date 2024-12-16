using DealW.Domain.Models;

namespace DealW.Domain.Abstractions;

public interface IQuizzesService
{
    Task<List<Quiz>> GetAllQuizzes();
    Task<int> CreateQuiz(Quiz quiz);
    Task<int> UpdateQuiz(int id, string title, string difficulty);
    Task<int> DeleteQuiz(int id);
}