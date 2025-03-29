using DealW.Domain.Models;

namespace DealW.Domain.Abstractions;

public interface IDuelsRepository
{
    Task<Duel> CreateDuelAsync(Duel duel);
    Task<Duel> GetDuelAsync(int duelId);
    Task<IEnumerable<Duel>> GetDuelsForUserAsync(Guid userId);
    Task UpdateDuelAsync(Duel duel);
}