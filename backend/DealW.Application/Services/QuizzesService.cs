using DealW.Domain.Abstractions;
using DealW.Domain.Models;

namespace DealW.Application.Services;

public class QuizzesService(IQuizzesRepository quizzesRepository) : IQuizzesService
{
    public async Task<List<Quiz>> GetAllQuizzes()
    {
        return await quizzesRepository.Get();
    }

    public async Task<int> CreateQuiz(Quiz quiz)
    {
        return await quizzesRepository.Create(quiz);
    }

    public async Task<int> UpdateQuiz(int id, string title, string difficulty)
    {
        return await quizzesRepository.Update(id, title, difficulty);
    }

    public async Task<int> DeleteQuiz(int id)
    {
        return await quizzesRepository.Delete(id);
    }
}