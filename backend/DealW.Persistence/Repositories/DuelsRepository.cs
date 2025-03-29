using AutoMapper;
using DealW.Domain.Abstractions;
using DealW.Domain.Models;
using DealW.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace DealW.Persistence.Repositories;

public class DuelsRepository(DealWDbContext context, IMapper mapper) : IDuelsRepository
{
    //TODO add mapper

    public async Task<Duel> CreateDuelAsync(Duel duel)
    {
        var duelEntity = mapper.Map<DuelEntity>(duel);
        await context.Duels.AddAsync(duelEntity);
        await context.SaveChangesAsync();
        return mapper.Map<Duel>(duelEntity);
    }

    public async Task<Duel> GetDuelAsync(int duelId)
    {
        var duelEntity = await context.Duels
            .Include(d => d.Questions)
            .ThenInclude(q => q.Answers)
            .FirstOrDefaultAsync(d => d.Id == duelId);

        return mapper.Map<Duel>(duelEntity);
    }

    public async Task<IEnumerable<Duel>> GetDuelsForUserAsync(Guid userId)
    {
        var duelEntities = await context.Duels
            .Where(d => d.User1Id == userId || d.User2Id == userId)
            .ToListAsync();

        return mapper.Map<IEnumerable<Duel>>(duelEntities);
    }

    public async Task UpdateDuelAsync(Duel duel)
    {
        var duelEntity = await context.Duels.FindAsync(duel.Id);
        if (duelEntity == null) throw new Exception("Duel not found");

        mapper.Map(duel, duelEntity);
        await context.SaveChangesAsync();
    }
}