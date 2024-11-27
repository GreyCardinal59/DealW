using DealW.Domain.Models;

namespace DealW.Persistence.Repositories;

public interface IQuizzesRepository
{
    Task<List<Quiz>> Get();
    Task<Guid> Create(Quiz quiz);
    Task<Guid> Update(Guid id, string title, IList<string> questions);
    Task<Guid> Delete(Guid id);
}