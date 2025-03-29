using AutoMapper;
using DealW.Domain.Abstractions;
using DealW.Domain.Models;
using DealW.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace DealW.Persistence.Repositories;

public class AnswersRepository(DealWDbContext context, IMapper mapper) : IAnswersRepository
{
    public async Task<Answer> SubmitAnswerAsync(Answer answer)
    {
        var answerEntity = mapper.Map<AnswerEntity>(answer);
        await context.Answers.AddAsync(answerEntity);
        await context.SaveChangesAsync();
        return mapper.Map<Answer>(answerEntity);
    }

    public async Task<IEnumerable<Answer>> GetAnswersForDuelQuestionAsync(int duelQuestionId)
    {
        var answerEntities = await context.Answers
            .Where(a => a.DuelQuestionId == duelQuestionId)
            .ToListAsync();

        return mapper.Map<IEnumerable<Answer>>(answerEntities);
    }
}