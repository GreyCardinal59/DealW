using DealW.Domain.Models;

namespace DealW.Domain.Abstractions;

public interface IQuizzesRepository
{
    Task<List<Quiz>> Get();
    Task<int> Create(Quiz quiz);
    Task<int> Update(int id, string title, string difficulty);
    Task<int> Delete(int id);
}