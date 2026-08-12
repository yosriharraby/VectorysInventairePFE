using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VectorysInventaire.Application.Interfaces.Services;
using VectorysInventaire.Domain.Entities;
using VectorysInventaire.Domain.Enums;

namespace VectorysInventaire.Infrastructure.Persistence.Seeders;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(VectorysDbContext db, IPasswordHasher passwordHasher, ILogger logger,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Vérification / seeding des utilisateurs de développement…");

        var desired = new List<Utilisateur>
        {
            new()
            {
                Nom = "Admin",
                Prenom = "Système",
                Email = "admin@vectorys.local",
                CodePersonnel = null,
                PasswordHash = passwordHasher.Hash("Admin123!"),
                Role = RoleEnum.ADMIN,
                Actif = true,
                CreatedAt = DateTime.UtcNow
            },
            // --- 3 superviseurs ---
            new()
            {
                Nom = "Ben Salah",
                Prenom = "Marwen",
                Email = "marwen.bensalah@vectorys.local",
                CodePersonnel = null,
                PasswordHash = passwordHasher.Hash("Super123!"),
                Role = RoleEnum.SUPERVISEUR,
                Actif = true,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Nom = "Zrelli",
                Prenom = "Siwar",
                Email = "siwar.zrelli@vectorys.local",
                CodePersonnel = null,
                PasswordHash = passwordHasher.Hash("Super123!"),
                Role = RoleEnum.SUPERVISEUR,
                Actif = true,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Nom = "Ferchichi",
                Prenom = "Karim",
                Email = "karim.ferchichi@vectorys.local",
                CodePersonnel = null,
                PasswordHash = passwordHasher.Hash("Super123!"),
                Role = RoleEnum.SUPERVISEUR,
                Actif = true,
                CreatedAt = DateTime.UtcNow
            },
            // Alias historique (même personne que Marwen)
            new()
            {
                Nom = "Ben Salah",
                Prenom = "Marwen",
                Email = "superviseur@vectorys.local",
                CodePersonnel = null,
                PasswordHash = passwordHasher.Hash("Super123!"),
                Role = RoleEnum.SUPERVISEUR,
                Actif = true,
                CreatedAt = DateTime.UtcNow
            },
            // --- Agents terrain ---
            new()
            {
                Nom = "Trabelsi",
                Prenom = "Sami",
                Email = null,
                CodePersonnel = "AG001",
                PasswordHash = passwordHasher.Hash("AG001"),
                Role = RoleEnum.AGENT,
                Actif = true,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Nom = "Mansouri",
                Prenom = "Nour",
                Email = null,
                CodePersonnel = "AG002",
                PasswordHash = passwordHasher.Hash("AG002"),
                Role = RoleEnum.AGENT,
                Actif = true,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Nom = "Jebali",
                Prenom = "Hedi",
                Email = null,
                CodePersonnel = "AG003",
                PasswordHash = passwordHasher.Hash("AG003"),
                Role = RoleEnum.AGENT,
                Actif = true,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Nom = "Gharbi",
                Prenom = "Amira",
                Email = null,
                CodePersonnel = "AG004",
                PasswordHash = passwordHasher.Hash("AG004"),
                Role = RoleEnum.AGENT,
                Actif = true,
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Nom = "Bouazizi",
                Prenom = "Yassine",
                Email = null,
                CodePersonnel = "AG005",
                PasswordHash = passwordHasher.Hash("AG005"),
                Role = RoleEnum.AGENT,
                Actif = true,
                CreatedAt = DateTime.UtcNow
            }
        };

        var added = 0;
        foreach (var user in desired)
        {
            var exists = user.Email != null
                ? await db.Utilisateurs.AnyAsync(u => u.Email == user.Email, cancellationToken)
                : await db.Utilisateurs.AnyAsync(u => u.CodePersonnel == user.CodePersonnel, cancellationToken);

            if (exists)
            {
                // Met à jour profil + mot de passe des comptes connus
                if (user.Email != null)
                {
                    var existing = await db.Utilisateurs.FirstAsync(u => u.Email == user.Email, cancellationToken);
                    existing.Nom = user.Nom;
                    existing.Prenom = user.Prenom;
                    existing.Role = user.Role;
                    existing.Actif = true;
                    existing.PasswordHash = user.PasswordHash;
                    existing.CodePersonnel = user.CodePersonnel;
                }
                else if (user.CodePersonnel != null)
                {
                    var existing = await db.Utilisateurs.FirstAsync(u => u.CodePersonnel == user.CodePersonnel, cancellationToken);
                    existing.Nom = user.Nom;
                    existing.Prenom = user.Prenom;
                    existing.Role = user.Role;
                    existing.Actif = true;
                    existing.PasswordHash = user.PasswordHash;
                    existing.Email = user.Email;
                }
                continue;
            }

            db.Utilisateurs.Add(user);
            added++;
        }

        await db.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Seeding utilisateurs terminé ({Added} nouveau(x)).", added);
    }
}
