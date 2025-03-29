using DealW.Domain.Models;
using DealW.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DealW.Persistence.Configurations;

public partial class DuelConfiguration : IEntityTypeConfiguration<DuelEntity>
{
    public void Configure(EntityTypeBuilder<DuelEntity> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(d => d.User1Id)
            .IsRequired();
        
        builder.Property(d => d.User2Id)
            .IsRequired();
        
        builder.Property(d => d.StartTime)
            .IsRequired();
        
        builder.Property(d => d.WinnerId)
            .IsRequired(false);
        
        builder.Property(d => d.Status)
            .IsRequired();
        
        builder.HasMany(d => d.Questions)
            .WithOne(q => q.Duel)
            .HasForeignKey(dq => dq.DuelId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(d => d.Player1)
            .WithMany()
            .HasForeignKey(d => d.User1Id)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(d => d.Player2)
            .WithMany()
            .HasForeignKey(d => d.User2Id)
            .OnDelete(DeleteBehavior.Restrict);
    }
}