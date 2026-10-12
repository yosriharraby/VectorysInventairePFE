using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VectorysInventaire.Application.Interfaces.Repositories;
using VectorysInventaire.Application.Interfaces.Services;
using VectorysInventaire.Domain.Interfaces.Repositories;
using VectorysInventaire.Infrastructure.Persistence;
using VectorysInventaire.Infrastructure.Persistence.Repositories;
using VectorysInventaire.Infrastructure.Services;

namespace VectorysInventaire.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<VectorysDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IUtilisateurRepository, UtilisateurRepository>();
        services.AddScoped<IInventaireRepository, InventaireRepository>();
        services.AddScoped<IPassageRepository, PassageRepository>();
        services.AddScoped<IEmplacementRepository, EmplacementRepository>();
        services.AddScoped<IStockTheoriqueRepository, StockTheoriqueRepository>();
        services.AddScoped<IScanRepository, ScanRepository>();
        services.AddScoped<IEcartRepository, EcartRepository>();
        services.AddScoped<IRecomptageRepository, RecomptageRepository>();
        services.AddScoped<IKpiRepository, KpiRepository>();

        services.AddSingleton<IPasswordHasher, BcryptPasswordHasher>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IExcelImportService, ExcelImportService>();
        services.AddScoped<IInventaireExportService, InventaireExportService>();
        services.AddScoped<IStockImportService, StockImportService>();
        services.AddScoped<IRecomptageProgressService, RecomptageProgressService>();

        return services;
    }
}
