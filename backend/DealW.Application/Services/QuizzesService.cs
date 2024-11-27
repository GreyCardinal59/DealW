using DealW.Domain.Models;
using DealW.Persistence.Repositories;

namespace DealW.Application.Services;

public class QuizzesService : IQuizzesService
{
    private readonly IQuizzesRepository _quizzesRepository;
    
    public QuizzesService(IQuizzesRepository quizzesRepository)
    {
        _quizzesRepository = quizzesRepository;
    }

    public async Task<List<Quiz>> GetAllQuizzes()
    {
        return await _quizzesRepository.Get();
    }

    public async Task<Guid> CreateQuiz(Quiz quiz)
    {
        return await _quizzesRepository.Create(quiz);
    }

    public async Task<Guid> UpdateQuiz(Guid id, string title, IList<string> questions)
    {
        return await _quizzesRepository.Update(id, title, questions);
    }

    public async Task<Guid> DeleteQuiz(Guid id)
    {
        return await _quizzesRepository.Delete(id);
    }
}