using DealW.Domain.Abstractions;
using DealW.Domain.Models;
using DealW.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace DealW.Persistence.Repositories;

public class QuestionsRepository(DealWDbContext context) : IQuestionsRepository
{
    // public async Task<List<Question>> Get()
    // {
    //     var questionEntities = await context.Questions
    //         .AsNoTracking()
    //         .ToListAsync();
    //     
    //     var questions = questionEntities
    //         .Select(q => Question.Create(q.Id, q.QuizId, q.Text, q.CorrectAnswerId).Question)
    //         .ToList();
    //     
    //     return questions;
    // }
    
    //TODO add mapper

    public async Task<List<Question>> GetByQuizId(int quizId)
    {
        var questionEntities = await context.Questions
            .Where(q => q.QuizId == quizId)
            .ToListAsync();

        var questions = questionEntities
            .Select(q => Question.Create(q.Id, q.QuizId, q.Text, q.CorrectAnswerId).Question)
            .ToList();
        
        return questions;
    }
    
    public async Task<int> Create(Question question) 
    {
        var questionEntity = new QuestionEntity
        {
            Id = question.Id,
            QuizId = question.QuizId,
            Text = question.Text,
            CorrectAnswerId = question.CorrectAnswerId
        };
        await context.Questions.AddAsync(questionEntity); 
        await context.SaveChangesAsync();
        
        return questionEntity.Id;
    }

    public async Task<int> Update(int id, string text, int correctAnswerId)
    {
        await context.Questions
            .Where(q => q.Id == id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(q => q.Text, q => text)
                .SetProperty(q => q.CorrectAnswerId, q => correctAnswerId));
        
        return id;
    }

    public async Task<int> Delete(int id)
    {
        await context.Questions
            .Where(q => q.Id == id)
            .ExecuteDeleteAsync();
        
        return id;
    }
    
}