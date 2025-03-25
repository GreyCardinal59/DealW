using DealW.Domain.Abstractions;
using DealW.Domain.Models;
using DealW.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace DealW.Persistence.Repositories;

public class QuizzesRepository(DealWDbContext context) : IQuizzesRepository
{
    //TODO add mapper
    
    public async Task<List<Quiz>> Get()
    {
        var quizEntities = await context.Quizzes
            .AsNoTracking()
            .ToListAsync();
        
        var quizzes = quizEntities
            .Select(q => Quiz.Create(q.Id, q.Title, q.Difficulty).Quiz)
            .ToList();
        
        return quizzes;
    }

    public async Task<int> Create(Quiz quiz)
    {
        var quizEntity = new QuizEntity
        {
            Id = quiz.Id,
            Title = quiz.Title,
            Difficulty = quiz.Difficulty,
        };
        await context.Quizzes.AddAsync(quizEntity); 
        await context.SaveChangesAsync();
        
        return quizEntity.Id;
    }

    public async Task<int> Update(int id, string title, string difficulty)
    {
        await context.Quizzes
            .Where(q => q.Id == id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(q => q.Title, q => title)
                .SetProperty(q => q.Difficulty, q => difficulty));
        
        return id;
    }

    public async Task<int> Delete(int id)
    {
        await context.Quizzes
            .Where(q => q.Id == id)
            .ExecuteDeleteAsync();
        
        return id;
    }
}