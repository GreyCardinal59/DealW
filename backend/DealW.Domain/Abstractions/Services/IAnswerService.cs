using DealW.Domain.Models;

namespace DealW.Domain.Abstractions;

public interface IAnswerService
{
    Task<Answer> SubmitAnswerAsync(int duelQuestionId, Guid userId, string userAnswer);
    Task<IEnumerable<Answer>> GetAnswersForDuelQuestionAsync(int duelQuestionId);
}