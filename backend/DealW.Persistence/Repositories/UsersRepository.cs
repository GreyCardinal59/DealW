using AutoMapper;
using DealW.Domain.Abstractions;
using DealW.Domain.Enums;
using DealW.Domain.Models;
using DealW.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace DealW.Persistence.Repositories;

public class UsersRepository(DealWDbContext context, IMapper mapper) : IUsersRepository
{
    //TODO add mapper
    public async Task Add(User user)
    {
        // Хардкод для проверки разрешений
        var roleEntity = await context.Roles
            .SingleOrDefaultAsync(r => r.Id == (int)Role.Admin)
                         ?? throw new InvalidOperationException();
        
        var userEntity = new UserEntity()
        {
            Id = user.Id,
            UserName = user.UserName,
            PasswordHash = user.PasswordHash,
            Email = user.Email,
            Roles = [roleEntity]
        };

        await context.Users.AddAsync(userEntity);
        await context.SaveChangesAsync();
    }

    public async Task<User> GetByEmail(string email)
    {
        var userEntity = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email) ?? throw new Exception();
        
        return mapper.Map<User>(userEntity);
    }
    
    public async Task<HashSet<Permission>> GetUserPermissions(Guid userId)
    {
        var roles = await context.Users
            .AsNoTracking()
            .Include(u => u.Roles)
            .ThenInclude(r => r.Permissions)
            .Where(u => u.Id == userId)
            .Select(u => u.Roles)
            .ToListAsync();
        
        return roles
            .SelectMany(r => r)
            .SelectMany(r => r.Permissions)
            .Select(p => (Permission)p.Id)
            .ToHashSet();
    }
}