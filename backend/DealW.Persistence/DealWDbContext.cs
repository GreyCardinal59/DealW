using DealW.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace DealW.Persistence;

public class DealWDbContext(DbContextOptions<DealWDbContext> options) : DbContext(options)
{
    public DbSet<QuizEntity> Quizzes { get; set; }
    public DbSet<QuestionEntity> Questions { get; set; }
    // public DbSet<Answer> Answers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<QuestionEntity>()
            .HasOne(q => q.Quiz)
            .WithMany(q => q.Questions)
            .HasForeignKey(q => q.QuizId);
    }
}