using DealW.Domain.Models;

namespace DealW.Domain.Abstractions;

public interface IQuestionsService
{
    Task<Question> CreateQuestionAsync(string questionText, string correctAnswer, string? category = null);
    Task<Question> GetQuestionAsync(int questionId);
    Task<IEnumerable<Question>> GetAllQuestionsAsync();
    Task DeleteQuestionAsync(int questionId);
}