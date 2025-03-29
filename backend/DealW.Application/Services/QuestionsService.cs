using DealW.Domain.Abstractions;
using DealW.Domain.Models;

namespace DealW.Application.Services;

public class QuestionsService(IQuestionsRepository questionsRepository) : IQuestionsService
{
    public Task<Question> CreateQuestionAsync(string questionText, string correctAnswer, string? category = null)
    {
        throw new NotImplementedException();
    }

    public Task<Question> GetQuestionAsync(int questionId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Question>> GetAllQuestionsAsync()
    {
        throw new NotImplementedException();
    }

    public Task DeleteQuestionAsync(int questionId)
    {
        throw new NotImplementedException();
    }
}