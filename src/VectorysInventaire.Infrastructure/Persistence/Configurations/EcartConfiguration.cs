using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VectorysInventaire.Domain.Entities;

namespace VectorysInventaire.Infrastructure.Persistence.Configurations;

public class EcartConfiguration : IEntityTypeConfiguration<Ecart>
{
    public void Configure(EntityTypeBuilder<Ecart> builder)
    {
        builder.ToTable("Ecart");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.CodeEmplacement).HasMaxLength(64).IsRequired();
        builder.Property(x => x.DesignationEmplacement).HasMaxLength(200);
        builder.Property(x => x.CodeUS).HasMaxLength(64).IsRequired();
        builder.Property(x => x.DesignationUS).HasMaxLength(200);
        builder.HasIndex(x => new { x.InventaireId, x.EmplacementId, x.CodeUS, x.Statut });
        builder.HasIndex(x => new { x.InventaireId, x.DetecteAt });
        builder.HasIndex(x => new { x.Statut, x.DetecteAt });
        builder.HasOne(x => x.Inventaire).WithMany().HasForeignKey(x => x.InventaireId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Passage).WithMany().HasForeignKey(x => x.PassageId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Emplacement).WithMany().HasForeignKey(x => x.EmplacementId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
