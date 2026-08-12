using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VectorysInventaire.Application.Interfaces.Services;
using VectorysInventaire.Infrastructure.Persistence;
using VectorysInventaire.Infrastructure.Persistence.Seeders;
using VectorysInventaire.Infrastructure.Services;

const string connectionString =
    "Server=YOSRI\\SQLEXPRESS;Database=VectorysDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;";

var options = new DbContextOptionsBuilder<VectorysDbContext>()
    .UseSqlServer(connectionString)
    .Options;

await using var db = new VectorysDbContext(options);
IPasswordHasher hasher = new BcryptPasswordHasher();
using var loggerFactory = LoggerFactory.Create(b => b.SetMinimumLevel(LogLevel.Information));
var logger = loggerFactory.CreateLogger("SeedUsers");

await db.Database.MigrateAsync();
await DatabaseSeeder.SeedAsync(db, hasher, logger);

var users = await db.Utilisateurs
    .OrderBy(u => u.Role)
    .ThenBy(u => u.Email ?? u.CodePersonnel)
    .ToListAsync();

Console.WriteLine();
Console.WriteLine("=== Comptes en base ===");
foreach (var u in users)
{
    Console.WriteLine($"{u.Role,-12} | {u.Prenom} {u.Nom,-20} | email={u.Email ?? "-"} | code={u.CodePersonnel ?? "-"}");
}
