using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VectorysInventaire.Domain.Entities;

namespace VectorysInventaire.Infrastructure.Persistence.Configurations;

public class StockTheoriqueConfiguration : IEntityTypeConfiguration<StockTheorique>
{
    public void Configure(EntityTypeBuilder<StockTheorique> builder)
    {
        builder.ToTable("StockTheorique");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.CodeUS).HasMaxLength(64).IsRequired();
        builder.Property(x => x.DesignationUS).HasMaxLength(200);
        builder.Property(x => x.Client).HasMaxLength(128);
        builder.HasIndex(x => new { x.EmplacementId, x.CodeUS }).IsUnique();
        builder.HasOne(x => x.Emplacement).WithMany(e => e.StocksTheoriques).HasForeignKey(x => x.EmplacementId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
