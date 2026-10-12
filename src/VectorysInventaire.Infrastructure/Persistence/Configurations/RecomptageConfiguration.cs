using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VectorysInventaire.Domain.Entities;

namespace VectorysInventaire.Infrastructure.Persistence.Configurations;

public class RecomptageConfiguration : IEntityTypeConfiguration<Recomptage>
{
    public void Configure(EntityTypeBuilder<Recomptage> builder)
    {
        builder.ToTable("Recomptages");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Mode).HasMaxLength(16);
        builder.Property(x => x.AssignationMode).HasMaxLength(32);
        builder.Property(x => x.FichierNom).HasMaxLength(256);
        builder.Property(x => x.Statut).HasMaxLength(32);
        builder.HasIndex(x => new { x.InventaireId, x.CreatedAt });

        builder.HasOne(x => x.Inventaire).WithMany().HasForeignKey(x => x.InventaireId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.PassageC2).WithMany().HasForeignKey(x => x.PassageC2Id)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Superviseur).WithMany().HasForeignKey(x => x.SuperviseurId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class RecomptageAgentConfiguration : IEntityTypeConfiguration<RecomptageAgent>
{
    public void Configure(EntityTypeBuilder<RecomptageAgent> builder)
    {
        builder.ToTable("RecomptageAgents");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.RecomptageId, x.AgentId }).IsUnique();

        builder.HasOne(x => x.Recomptage).WithMany(r => r.Agents).HasForeignKey(x => x.RecomptageId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Agent).WithMany().HasForeignKey(x => x.AgentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
