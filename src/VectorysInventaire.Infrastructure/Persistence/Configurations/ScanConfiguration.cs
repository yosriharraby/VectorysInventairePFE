using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VectorysInventaire.Domain.Entities;

namespace VectorysInventaire.Infrastructure.Persistence.Configurations;

public class ScanConfiguration : IEntityTypeConfiguration<Scan>
{
    public void Configure(EntityTypeBuilder<Scan> builder)
    {
        builder.ToTable("Scans");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.CodeBarresScan).HasMaxLength(256).IsRequired();
        builder.Property(x => x.IdLocalFlutter).HasMaxLength(64);
        builder.HasIndex(x => new { x.EmplacementId, x.DateHeureScan });
        builder.HasIndex(x => x.DateHeureScan);
        builder.HasIndex(x => x.IdLocalFlutter).IsUnique().HasFilter("[IdLocalFlutter] IS NOT NULL");
        builder.HasOne(x => x.Emplacement).WithMany(e => e.Scans).HasForeignKey(x => x.EmplacementId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Agent).WithMany().HasForeignKey(x => x.AgentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
