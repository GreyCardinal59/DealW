using DealW.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DealW.Persistence.Configurations;

public partial class AnswerConfiguration : IEntityTypeConfiguration<AnswerEntity>
{
    public void Configure(EntityTypeBuilder<AnswerEntity> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(a => a.DuelQuestionId)
            .IsRequired();
        
        builder.Property(a => a.UserId)
            .IsRequired();
        
        builder.Property(a => a.UserAnswer)
            .HasMaxLength(250) // Пример длины, можно изменить
            .IsRequired();
        
        builder.Property(a => a.IsCorrect)
            .IsRequired();
        
        builder.HasOne(a => a.DuelQuestion)
            .WithMany(dq => dq.Answers)
            .HasForeignKey(a => a.DuelQuestionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}