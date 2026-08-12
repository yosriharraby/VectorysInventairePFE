using VectorysInventaire.Application.DTOs;

namespace VectorysInventaire.Application.Interfaces.Services;

public interface IScanCompletionService
{
    Task EnsureScanQuotaAvailableAsync(int emplacementId, string codeUs, CancellationToken cancellationToken = default);
    Task<bool> IsStockCompleteAsync(int emplacementId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StockLigneProgressDto>> GetProgressAsync(int emplacementId,
        CancellationToken cancellationToken = default);
}
