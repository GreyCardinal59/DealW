using DealW.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quiz = DealW.Domain.Models.Quiz;

namespace DealW.Persistence.Configurations;

internal class QuizConfiguration : IEntityTypeConfiguration<QuizEntity>
{
    public void Configure(EntityTypeBuilder<QuizEntity> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(q => q.Title)
            .HasMaxLength(Quiz.MAX_TITLE_LENGHT)
            .IsRequired();
        
        builder.Property(q => q.Difficulty)
            .IsRequired();
    }
}