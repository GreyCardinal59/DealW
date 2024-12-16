using DealW.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DealW.Persistence.Configurations;

internal class QuestionConfiguration : IEntityTypeConfiguration<QuestionEntity>
{
    public void Configure(EntityTypeBuilder<QuestionEntity> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(q => q.QuizId)
            .IsRequired();
        
        builder.Property(q => q.Text)
            .IsRequired()
            .HasMaxLength(500);
        
        builder.Property(q => q.CorrectAnswerId)
            .IsRequired();
        
        builder.HasOne(q => q.Quiz)
            .WithMany(quiz => quiz.Questions)
            .HasForeignKey(q => q.QuizId);
    }
}