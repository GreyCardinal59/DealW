using DealW.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DealW.Persistence.Configurations;

public partial class QuestionConfiguration : IEntityTypeConfiguration<QuestionEntity>
{
    public void Configure(EntityTypeBuilder<QuestionEntity> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(q => q.Text)
            .HasMaxLength(500) // Пример длины, можно изменить
            .IsRequired();
        
        builder.Property(q => q.CorrectAnswer)
            .HasMaxLength(250) // Пример длины, можно изменить
            .IsRequired();
    }
}