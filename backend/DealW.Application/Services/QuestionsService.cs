using DealW.Domain.Abstractions;
using DealW.Domain.Models;

namespace DealW.Application.Services;

public class QuestionsService(IQuestionsRepository questionsRepository) : IQuestionsService
{
    // public async Task<List<Question>> GetAllQuestions()
    // {
    //     return await questionsRepository.Get();
    // }

    public async Task<List<Question>> GetByQuizId(int quizId)
    {
        return await questionsRepository.GetByQuizId(quizId);
    }

    public async Task<int> CreateQuestion(Question question)
    {
        return await questionsRepository.Create(question);
    }

    public async Task<int> UpdateQuestion(int id, string text, int correctAnswer)
    {
        return await questionsRepository.Update(id, text, correctAnswer);
    }

    public async Task<int> DeleteQuestion(int id)
    {
        return await questionsRepository.Delete(id);
    }
}