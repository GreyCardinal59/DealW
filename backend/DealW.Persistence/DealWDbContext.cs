using DealW.Persistence.Configurations;
using DealW.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DealW.Persistence;

public class DealWDbContext(DbContextOptions<DealWDbContext> options,
    IOptions<AuthorizationOptions> authOptions) : DbContext(options)
{
    public DbSet<QuizEntity> Quizzes { get; set; }
    public DbSet<QuestionEntity> Questions { get; set; }
    
    public DbSet<UserEntity> Users { get; set; }
    
    public DbSet<RoleEntity> Roles { get; set; }
    // public DbSet<Answer> Answers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DealWDbContext).Assembly);
        
        modelBuilder.ApplyConfiguration(new RolePermissionConfiguration(authOptions.Value));
        
        modelBuilder.Entity<QuestionEntity>()
            .HasOne(q => q.Quiz)
            .WithMany(q => q.Questions)
            .HasForeignKey(q => q.QuizId);
    }
}