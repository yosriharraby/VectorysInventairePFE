using VectorysInventaire.Application.DTOs;

namespace VectorysInventaire.Application.Interfaces.Services;

public interface IStockImportService
{
    Task<ImportStockResultDto> ImportForPassageAsync(int passageId, Stream fileStream,
        IReadOnlyList<string>? defaultCodeAgents = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Branche B : crée/obtient le passage C2, importe l'Excel (GLOBAL ou CIBLE),
    /// enregistre un Recomptage en attente. La correction des écarts est faite par l'agent.
    /// </summary>
    Task<AppliquerBrancheBResultDto> ImportBrancheBAsync(
        int inventaireId,
        Stream fileStream,
        string mode,
        string? fichierNom = null,
        string? assignationMode = null,
        string? agentCode = null,
        int? superviseurId = null,
        CancellationToken cancellationToken = default);

    Task<byte[]> ExportC1StockAsync(int inventaireId, CancellationToken cancellationToken = default);

    Task<(byte[] Content, string FileName)> ExportRecomptageAsync(int recomptageId, CancellationToken cancellationToken = default);
}
