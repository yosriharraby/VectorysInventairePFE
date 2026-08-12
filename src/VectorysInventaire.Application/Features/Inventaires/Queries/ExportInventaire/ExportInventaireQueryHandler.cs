using MediatR;
using VectorysInventaire.Application.Interfaces.Services;
using VectorysInventaire.Domain.Interfaces.Repositories;

namespace VectorysInventaire.Application.Features.Inventaires.Queries.ExportInventaire;

public class ExportInventaireQueryHandler : IRequestHandler<ExportInventaireQuery, (byte[] Content, string FileName)>
{
    private readonly IInventaireRepository _inventaireRepository;
    private readonly IInventaireExportService _exportService;

    public ExportInventaireQueryHandler(
        IInventaireRepository inventaireRepository,
        IInventaireExportService exportService)
    {
        _inventaireRepository = inventaireRepository;
        _exportService = exportService;
    }

    public async Task<(byte[] Content, string FileName)> Handle(
        ExportInventaireQuery request,
        CancellationToken cancellationToken)
    {
        var inventaire = await _inventaireRepository.GetByIdAsync(request.InventaireId, cancellationToken)
            ?? throw new KeyNotFoundException("Inventaire introuvable.");

        var bytes = await _exportService.ExportInventaireAsync(request.InventaireId, cancellationToken);
        var fileName = $"Inventaire_{inventaire.Reference}_{DateTime.UtcNow:yyyyMMdd_HHmm}.xlsx";
        return (bytes, fileName);
    }
}
