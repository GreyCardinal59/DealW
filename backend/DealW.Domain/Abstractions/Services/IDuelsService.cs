using DealW.Domain.Models;

namespace DealW.Domain.Abstractions;

public interface IDuelsService
{
    Task<Duel> CreateDuelAsync(Guid player1Id, Guid player2Id);
    Task<Duel> GetDuelAsync(int duelId);
    Task<IEnumerable<Duel>> GetDuelsForUserAsync(Guid userId);
    Task StartDuelAsync(int duelId);
    Task EndDuelAsync(int duelId, Guid winnerId);
}