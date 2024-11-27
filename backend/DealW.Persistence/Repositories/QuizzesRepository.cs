using DealW.Domain.Models;
using DealW.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace DealW.Persistence.Repositories;

public class QuizzesRepository : IQuizzesRepository
{
    private readonly DealWDbContext _context;

    public QuizzesRepository(DealWDbContext context)
    {
        _context = context;
    }

    public async Task<List<Quiz>> Get()
    {
        var quizEntities = await _context.Quizzes
            .AsNoTracking()
            .ToListAsync();
        
        var quizzes = quizEntities
            .Select(q => Quiz.Create(q.Id, q.Title, q.Questions).Quiz)
            .ToList();
        
        return quizzes;
    }

    public async Task<Guid> Create(Quiz quiz)
    {
        var quizEntity = new QuizEntity
        {
            Id = quiz.Id,
            Title = quiz.Title,
            Questions = quiz.Questions
        };
        await _context.Quizzes.AddAsync(quizEntity); 
        await _context.SaveChangesAsync();
        
        return quizEntity.Id;
    }

    public async Task<Guid> Update(Guid id, string title, IList<string> questions)
    {
        await _context.Quizzes
            .Where(q => q.Id == id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(q => q.Title, q => title)
                .SetProperty(q => q.Questions, q => questions));
        
        return id;
    }

    public async Task<Guid> Delete(Guid id)
    {
        await _context.Quizzes
            .Where(q => q.Id == id)
            .ExecuteDeleteAsync();
        
        return id;
    }
}