using VectorysInventaire.Application.DTOs;

namespace VectorysInventaire.Application.Interfaces.Services;

public interface IStockImportService
{
    Task<ImportStockResultDto> ImportForPassageAsync(int passageId, Stream fileStream,
        IReadOnlyList<string>? defaultCodeAgents = null,
        CancellationToken cancellationToken = default);
}
