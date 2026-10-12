using VectorysInventaire.Application.DTOs;
using VectorysInventaire.Application.Interfaces.Services;
using VectorysInventaire.Domain.Interfaces.Repositories;

namespace VectorysInventaire.Application.Services;

public class ScanCompletionService : IScanCompletionService
{
    private readonly IScanRepository _scanRepository;
    private readonly IStockTheoriqueRepository _stockRepository;

    public ScanCompletionService(IScanRepository scanRepository, IStockTheoriqueRepository stockRepository)
    {
        _scanRepository = scanRepository;
        _stockRepository = stockRepository;
    }

    public async Task EnsureScanQuotaAvailableAsync(int emplacementId, string codeUs,
        CancellationToken cancellationToken = default)
    {
        var normalized = codeUs.Trim();
        var qteTheorique = await _stockRepository.GetQteTheoriqueForCodeUsAsync(
            emplacementId, normalized, cancellationToken);

        if (qteTheorique is null or <= 0)
            throw new InvalidOperationException($"Code-barres « {normalized} » inconnu pour cet emplacement.");

        var qteScannee = await _scanRepository.CountByEmplacementIdAndCodeBarresAsync(
            emplacementId, normalized, cancellationToken);

        if (qteScannee >= qteTheorique.Value)
            throw new InvalidOperationException(
                $"Quota atteint pour le code US « {normalized} » ({qteTheorique.Value} scan(s) attendu(s)).");
    }

    public async Task<bool> IsStockCompleteAsync(int emplacementId, CancellationToken cancellationToken = default)
    {
        var stocks = await _stockRepository.GetByEmplacementIdAsync(emplacementId, cancellationToken);
        if (stocks.Count == 0)
            return false;

        foreach (var stock in stocks)
        {
            var qteScannee = await _scanRepository.CountByEmplacementIdAndCodeBarresAsync(
                emplacementId, stock.CodeUS, cancellationToken);
            if (qteScannee < stock.QteTheorique)
                return false;
        }

        return true;
    }

    public async Task<IReadOnlyList<StockLigneProgressDto>> GetProgressAsync(int emplacementId,
        CancellationToken cancellationToken = default)
    {
        var stocks = await _stockRepository.GetByEmplacementIdAsync(emplacementId, cancellationToken);
        var progress = new List<StockLigneProgressDto>(stocks.Count);

        foreach (var stock in stocks)
        {
            var qteScannee = await _scanRepository.CountByEmplacementIdAndCodeBarresAsync(
                emplacementId, stock.CodeUS, cancellationToken);
            progress.Add(new StockLigneProgressDto(
                stock.CodeUS,
                stock.DesignationUS,
                stock.QteTheorique,
                qteScannee,
                stock.Client));
        }

        return progress;
    }
}
