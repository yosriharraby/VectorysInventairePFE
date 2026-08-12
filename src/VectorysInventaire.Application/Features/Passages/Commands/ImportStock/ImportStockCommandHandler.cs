using MediatR;
using VectorysInventaire.Application.DTOs;
using VectorysInventaire.Application.Interfaces.Services;

namespace VectorysInventaire.Application.Features.Passages.Commands.ImportStock;

public class ImportStockCommandHandler : IRequestHandler<ImportStockCommand, ImportStockResultDto>
{
    private readonly IStockImportService _stockImportService;

    public ImportStockCommandHandler(IStockImportService stockImportService)
    {
        _stockImportService = stockImportService;
    }

    public Task<ImportStockResultDto> Handle(ImportStockCommand request, CancellationToken cancellationToken)
        => _stockImportService.ImportForPassageAsync(
            request.PassageId,
            request.FileStream,
            request.DefaultCodeAgents,
            cancellationToken);
}
