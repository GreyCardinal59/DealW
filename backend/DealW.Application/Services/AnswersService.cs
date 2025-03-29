using DealW.Domain.Abstractions;
using DealW.Domain.Models;

namespace DealW.Application.Services;

public class AnswersService(IAnswersRepository answersRepository) : IAnswerService
{
    public Task<Answer> SubmitAnswerAsync(int duelQuestionId, Guid userId, string userAnswer)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Answer>> GetAnswersForDuelQuestionAsync(int duelQuestionId)
    {
        throw new NotImplementedException();
    }
}