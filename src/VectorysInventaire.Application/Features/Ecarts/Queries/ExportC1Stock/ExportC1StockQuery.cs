using MediatR;
using VectorysInventaire.Application.Interfaces.Services;

namespace VectorysInventaire.Application.Features.Ecarts.Queries.ExportC1Stock;

public record ExportC1StockQuery(int InventaireId) : IRequest<(byte[] Content, string FileName)>;

public class ExportC1StockQueryHandler : IRequestHandler<ExportC1StockQuery, (byte[] Content, string FileName)>
{
    private readonly IStockImportService _stockImportService;

    public ExportC1StockQueryHandler(IStockImportService stockImportService) =>
        _stockImportService = stockImportService;

    public async Task<(byte[] Content, string FileName)> Handle(
        ExportC1StockQuery request,
        CancellationToken cancellationToken)
    {
        var bytes = await _stockImportService.ExportC1StockAsync(request.InventaireId, cancellationToken);
        return (bytes, $"inventaire-{request.InventaireId}-c1-stock.xlsx");
    }
}
