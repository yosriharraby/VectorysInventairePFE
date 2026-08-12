using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VectorysInventaire.Domain.Entities;

namespace VectorysInventaire.Infrastructure.Persistence.Configurations;

public class PassageConfiguration : IEntityTypeConfiguration<Passage>
{
    public void Configure(EntityTypeBuilder<Passage> builder)
    {
        builder.ToTable("Passages");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Type).HasConversion<string>().HasMaxLength(8);
        builder.Property(x => x.Statut).HasMaxLength(32);
        builder.HasIndex(x => new { x.InventaireId, x.Type }).IsUnique();
        builder.HasOne(x => x.Inventaire).WithMany(i => i.Passages).HasForeignKey(x => x.InventaireId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
