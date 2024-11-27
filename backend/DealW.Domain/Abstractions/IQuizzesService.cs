using DealW.Domain.Models;

namespace DealW.Application.Services;

public interface IQuizzesService
{
    Task<List<Quiz>> GetAllQuizzes();
    Task<Guid> CreateQuiz(Quiz quiz);
    Task<Guid> UpdateQuiz(Guid id, string title, IList<string> questions);
    Task<Guid> DeleteQuiz(Guid id);
}