using DealW.Domain.Models;

namespace DealW.Domain.Abstractions;

public interface IAnswersRepository
{
    Task<Answer> SubmitAnswerAsync(Answer answer);
    Task<IEnumerable<Answer>> GetAnswersForDuelQuestionAsync(int duelQuestionId);
}