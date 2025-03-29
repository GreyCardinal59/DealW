using DealW.Domain.Abstractions;
using DealW.Domain.Models;

namespace DealW.Application.Services;

public class DuelsService(IDuelsRepository duelsRepository) : IDuelsService
{
    public Task<Duel> CreateDuelAsync(Guid player1Id, Guid player2Id)
    {
        throw new NotImplementedException();
    }

    public Task<Duel> GetDuelAsync(int duelId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Duel>> GetDuelsForUserAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task StartDuelAsync(int duelId)
    {
        throw new NotImplementedException();
    }

    public Task EndDuelAsync(int duelId, Guid winnerId)
    {
        throw new NotImplementedException();
    }
}