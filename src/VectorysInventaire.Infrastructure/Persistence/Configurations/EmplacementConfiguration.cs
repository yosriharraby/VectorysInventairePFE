using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VectorysInventaire.Domain.Entities;

namespace VectorysInventaire.Infrastructure.Persistence.Configurations;

public class EmplacementConfiguration : IEntityTypeConfiguration<Emplacement>
{
    public void Configure(EntityTypeBuilder<Emplacement> builder)
    {
        builder.ToTable("Emplacements");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Code).HasMaxLength(64).IsRequired();
        builder.Property(x => x.Designation).HasMaxLength(200);
        builder.Property(x => x.CodeBarres).HasMaxLength(64);
        builder.Property(x => x.Statut).HasConversion<string>().HasMaxLength(32);
        builder.HasIndex(x => new { x.PassageId, x.AgentAssigneId });
        builder.HasOne(x => x.Passage).WithMany(p => p.Emplacements).HasForeignKey(x => x.PassageId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.AgentAssigne).WithMany().HasForeignKey(x => x.AgentAssigneId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
