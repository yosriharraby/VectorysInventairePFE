using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VectorysInventaire.Domain.Entities;

namespace VectorysInventaire.Infrastructure.Persistence.Configurations;

public class InventaireConfiguration : IEntityTypeConfiguration<Inventaire>
{
    public void Configure(EntityTypeBuilder<Inventaire> builder)
    {
        builder.ToTable("Inventaires");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Reference).HasMaxLength(32).IsRequired();
        builder.HasIndex(x => x.Reference).IsUnique();
        builder.Property(x => x.Intitule).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Zone).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Statut).HasConversion<string>().HasMaxLength(32);
        builder.HasOne(x => x.Superviseur).WithMany().HasForeignKey(x => x.SuperviseurId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
