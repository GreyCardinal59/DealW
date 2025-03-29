using DealW.Domain.Models;

namespace DealW.Domain.Abstractions;

public interface IQuestionsRepository
{
    Task<Question> GetQuestionAsync(int questionId);
    Task<IEnumerable<Question>> GetQuestionsAsync();
}