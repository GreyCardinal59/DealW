using DealW.Domain.Models;

namespace DealW.Domain.Abstractions;

public interface IQuestionsRepository
{
    // Task<List<Question>> Get();
    Task<List<Question>> GetByQuizId(int quizId);
    Task<int> Create(Question question);
    Task<int> Update(int id, string text, int correctAnswer);
    Task<int> Delete(int id);
}