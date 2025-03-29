using DealW.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DealW.Persistence.Configurations;

public partial class DuelQuestionConfiguration : IEntityTypeConfiguration<DuelQuestionEntity>
{
    public void Configure(EntityTypeBuilder<DuelQuestionEntity> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(dq => dq.DuelId)
            .IsRequired();
        
        builder.Property(dq => dq.QuestionId)
            .IsRequired();
        

        builder.HasOne(dq => dq.Duel)
            .WithMany(d => d.Questions)
            .HasForeignKey(dq => dq.DuelId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(dq => dq.Question)
            .WithMany(q => q.DuelQuestions)
            .HasForeignKey(dq => dq.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}