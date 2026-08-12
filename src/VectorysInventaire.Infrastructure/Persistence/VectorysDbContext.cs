using Microsoft.EntityFrameworkCore;
using VectorysInventaire.Domain.Entities;

namespace VectorysInventaire.Infrastructure.Persistence;

public class VectorysDbContext : DbContext
{
    public VectorysDbContext(DbContextOptions<VectorysDbContext> options)
        : base(options)
    {
    }

    public DbSet<Utilisateur> Utilisateurs => Set<Utilisateur>();
    public DbSet<Inventaire> Inventaires => Set<Inventaire>();
    public DbSet<Passage> Passages => Set<Passage>();
    public DbSet<Emplacement> Emplacements => Set<Emplacement>();
    public DbSet<StockTheorique> StocksTheoriques => Set<StockTheorique>();
    public DbSet<Scan> Scans => Set<Scan>();
    public DbSet<Ecart> Ecarts => Set<Ecart>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(VectorysDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
