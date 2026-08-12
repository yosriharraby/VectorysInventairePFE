namespace VectorysInventaire.Application.Interfaces.Services;

public interface IInventaireExportService
{
    Task<byte[]> ExportInventaireAsync(int inventaireId, CancellationToken cancellationToken = default);
}
