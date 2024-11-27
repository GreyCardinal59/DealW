using DealW.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace DealW.Persistence;

public class DealWDbContext : DbContext
{
    public DealWDbContext(DbContextOptions<DealWDbContext> options)
        : base(options)
    {
        
    }
    
    public DbSet<QuizEntity> Quizzes { get; set; }
}