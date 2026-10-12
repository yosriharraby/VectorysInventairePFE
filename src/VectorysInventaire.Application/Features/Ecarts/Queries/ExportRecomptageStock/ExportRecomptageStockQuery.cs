using MediatR;
using VectorysInventaire.Application.Interfaces.Services;

namespace VectorysInventaire.Application.Features.Ecarts.Queries.ExportRecomptageStock;

public record ExportRecomptageStockQuery(int RecomptageId) : IRequest<(byte[] Content, string FileName)>;

public class ExportRecomptageStockQueryHandler
    : IRequestHandler<ExportRecomptageStockQuery, (byte[] Content, string FileName)>
{
    private readonly IStockImportService _stockImportService;

    public ExportRecomptageStockQueryHandler(IStockImportService stockImportService) =>
        _stockImportService = stockImportService;

    public Task<(byte[] Content, string FileName)> Handle(
        ExportRecomptageStockQuery request,
        CancellationToken cancellationToken) =>
        _stockImportService.ExportRecomptageAsync(request.RecomptageId, cancellationToken);
}
