using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VectorysInventaire.Domain.Entities;

namespace VectorysInventaire.Infrastructure.Persistence.Configurations;

public class UtilisateurConfiguration : IEntityTypeConfiguration<Utilisateur>
{
    public void Configure(EntityTypeBuilder<Utilisateur> builder)
    {
        builder.ToTable("Utilisateurs");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Nom).HasMaxLength(100).IsRequired();
        builder.Property(u => u.Prenom).HasMaxLength(100).IsRequired();
        builder.Property(u => u.CodePersonnel).HasMaxLength(32);
        builder.Property(u => u.Email).HasMaxLength(256);
        builder.Property(u => u.PasswordHash).HasMaxLength(512).IsRequired();
        builder.Property(u => u.Role)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.HasIndex(u => u.CodePersonnel).IsUnique().HasFilter("[CodePersonnel] IS NOT NULL");
        builder.HasIndex(u => u.Email).IsUnique().HasFilter("[Email] IS NOT NULL");
    }
}
