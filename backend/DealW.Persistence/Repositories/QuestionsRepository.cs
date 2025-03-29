using AutoMapper;
using DealW.Domain.Abstractions;
using DealW.Domain.Models;
using DealW.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace DealW.Persistence.Repositories;

public class QuestionsRepository(DealWDbContext _context, IMapper _mapper) : IQuestionsRepository
{
    public Task<Question> GetQuestionAsync(int questionId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Question>> GetQuestionsAsync()
    {
        throw new NotImplementedException();
    }
}