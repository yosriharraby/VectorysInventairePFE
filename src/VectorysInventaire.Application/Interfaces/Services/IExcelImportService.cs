namespace VectorysInventaire.Application.Interfaces.Services;

public record ExcelStockRow(
    int RowNumber,
    string CodeEmplacement,
    string? DesignationEmplacement,
    string CodeUS,
    string? DesignationUS,
    int QteTheorique,
    string? CodeAgentPersonnel);

public interface IExcelImportService
{
  Task<IReadOnlyList<ExcelStockRow>> ParseStockFileAsync(Stream stream, CancellationToken cancellationToken = default);
}
