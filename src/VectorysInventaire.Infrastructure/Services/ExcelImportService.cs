using ClosedXML.Excel;
using VectorysInventaire.Application.Interfaces.Services;

namespace VectorysInventaire.Infrastructure.Services;

public class ExcelImportService : IExcelImportService
{
    public Task<IReadOnlyList<ExcelStockRow>> ParseStockFileAsync(Stream stream,
        CancellationToken cancellationToken = default)
    {
        using var workbook = new XLWorkbook(stream);
        var worksheet = workbook.Worksheets.First();
        var rows = new List<ExcelStockRow>();
        var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;

        for (var r = 2; r <= lastRow; r++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var codeEmplacement = worksheet.Cell(r, 1).GetString().Trim();
            if (string.IsNullOrWhiteSpace(codeEmplacement))
                continue;

            var designationEmplacement = worksheet.Cell(r, 2).GetString().Trim();
            var codeUs = worksheet.Cell(r, 3).GetString().Trim();
            if (string.IsNullOrWhiteSpace(codeUs))
                continue;

            var designationUs = worksheet.Cell(r, 4).GetString().Trim();
            var qteCell = worksheet.Cell(r, 5);
            var qte = qteCell.TryGetValue<int>(out var q) ? q : (int)qteCell.GetDouble();
            var codeAgent = worksheet.Cell(r, 6).GetString().Trim();
            var client = worksheet.Cell(r, 7).GetString().Trim();

            if (qte < 0)
                continue;

            rows.Add(new ExcelStockRow(
                r,
                codeEmplacement,
                string.IsNullOrWhiteSpace(designationEmplacement) ? null : designationEmplacement,
                codeUs,
                string.IsNullOrWhiteSpace(designationUs) ? null : designationUs,
                qte,
                string.IsNullOrWhiteSpace(codeAgent) ? null : codeAgent,
                string.IsNullOrWhiteSpace(client) ? null : client));
        }

        return Task.FromResult<IReadOnlyList<ExcelStockRow>>(rows);
    }
}
