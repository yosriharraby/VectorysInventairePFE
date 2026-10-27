using System.Globalization;
using System.Text;
using ClosedXML.Excel;
using VectorysInventaire.Application.Interfaces.Services;

namespace VectorysInventaire.Infrastructure.Services;

public class ExcelImportService : IExcelImportService
{
    public Task<IReadOnlyList<ExcelStockRow>> ParseStockFileAsync(Stream stream,
        CancellationToken cancellationToken = default)
    {
        Stream readable = stream;
        if (!stream.CanSeek)
        {
            var copy = new MemoryStream();
            stream.CopyTo(copy);
            copy.Position = 0;
            readable = copy;
        }
        else if (stream.Position != 0 && stream.CanSeek)
        {
            stream.Position = 0;
        }

        using var workbook = new XLWorkbook(readable);
        var worksheet = workbook.Worksheets.FirstOrDefault(w => w.LastRowUsed() is not null)
                        ?? workbook.Worksheets.First();
        var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 1;
        var lastCol = Math.Max(worksheet.LastColumnUsed()?.ColumnNumber() ?? 7, 7);

        var map = DetectColumns(worksheet, lastCol);
        var startRow = map.HasHeaderRow ? 2 : 1;
        var rows = new List<ExcelStockRow>();

        for (var r = startRow; r <= lastRow; r++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var codeEmplacement = CellText(worksheet.Cell(r, map.CodeEmplacement));
            if (LooksLikeHeader(codeEmplacement) && r == startRow)
                continue;
            if (string.IsNullOrWhiteSpace(codeEmplacement))
                continue;

            var designationEmplacement = CellText(worksheet.Cell(r, map.DesignationEmplacement));
            var codeUs = CellText(worksheet.Cell(r, map.CodeUS));
            var designationUs = CellText(worksheet.Cell(r, map.DesignationUS));
            var qte = ParseQty(worksheet.Cell(r, map.QteTheorique));
            var codeAgent = CellText(worksheet.Cell(r, map.CodeAgent));
            var client = CellText(worksheet.Cell(r, map.Client));

            if (string.IsNullOrWhiteSpace(codeUs))
                codeUs = $"{codeEmplacement}#L{r}";

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

    private static ColumnMap DetectColumns(IXLWorksheet worksheet, int lastCol)
    {
        var map = new ColumnMap();
        var found = 0;

        for (var c = 1; c <= lastCol; c++)
        {
            var key = NormalizeHeader(CellText(worksheet.Cell(1, c)));
            if (key.Length == 0)
                continue;

            if (IsEmplacementCode(key)) { map.CodeEmplacement = c; found++; }
            else if (IsEmplacementDesignation(key)) { map.DesignationEmplacement = c; found++; }
            else if (IsUsCode(key)) { map.CodeUS = c; found++; }
            else if (IsUsDesignation(key)) { map.DesignationUS = c; found++; }
            else if (IsQty(key)) { map.QteTheorique = c; found++; }
            else if (IsAgent(key)) { map.CodeAgent = c; found++; }
            else if (IsClient(key)) { map.Client = c; found++; }
        }

        map.HasHeaderRow = found >= 2;
        return map;
    }

    private static bool LooksLikeHeader(string value)
    {
        var key = NormalizeHeader(value);
        return IsEmplacementCode(key) || key is "emplacement" or "code";
    }

    private static bool IsEmplacementCode(string key) =>
        key is "codeemplacement" or "emplacement" or "codeemp" or "location" or "bin"
            or "emplacementcode" or "codelocation";

    private static bool IsEmplacementDesignation(string key) =>
        key is "designationemplacement" or "libelleemplacement" or "emplacementlibelle"
            or "designationemp" or "descriptionemplacement";

    private static bool IsUsCode(string key) =>
        key is "codeus" or "us" or "sku" or "ean" or "gtin" or "barcode"
            or "codebarres" or "codebarre" or "article" or "codearticle" or "ref" or "reference";

    private static bool IsUsDesignation(string key) =>
        key is "designationus" or "libelleus" or "designationarticle" or "descriptionus"
            or "libellearticle" or "designation";

    private static bool IsQty(string key) =>
        key is "qtetheorique" or "qte" or "quantite" or "quantitetheorique"
            or "qty" or "quantity" or "stock" or "stocktheorique";

    private static bool IsAgent(string key) =>
        key is "codeagent" or "agent" or "codepersonnel" or "matricule" or "codeagentpersonnel";

    private static bool IsClient(string key) =>
        key is "client" or "donneurordre" or "customer";

    private static string NormalizeHeader(string value)
    {
        var formD = value.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder(formD.Length);
        foreach (var ch in formD)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(ch) == UnicodeCategory.NonSpacingMark)
                continue;
            if (char.IsLetterOrDigit(ch))
                sb.Append(char.ToLowerInvariant(ch));
        }
        return sb.ToString();
    }

    internal static string CellText(IXLCell cell)
    {
        if (cell.IsEmpty() || cell.DataType == XLDataType.Blank)
            return string.Empty;

        try
        {
            switch (cell.DataType)
            {
                case XLDataType.Number:
                    return FormatNumber(cell.GetDouble());
                case XLDataType.Boolean:
                    return cell.GetBoolean() ? "true" : "false";
                case XLDataType.DateTime:
                    return cell.GetDateTime().ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
                case XLDataType.TimeSpan:
                    return cell.GetTimeSpan().ToString();
                default:
                    var formatted = cell.GetFormattedString()?.Trim() ?? string.Empty;
                    if (formatted.Length > 0 && formatted.IndexOf('E') < 0 && formatted.IndexOf('e') < 0)
                        return formatted;

                    var raw = cell.GetString()?.Trim() ?? string.Empty;
                    if (double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out var n)
                        || double.TryParse(raw, NumberStyles.Float, CultureInfo.CurrentCulture, out n))
                        return FormatNumber(n);

                    return raw.Length > 0 ? raw : formatted;
            }
        }
        catch
        {
            return (cell.GetFormattedString() ?? cell.GetString() ?? string.Empty).Trim();
        }
    }

    private static string FormatNumber(double n)
    {
        if (double.IsNaN(n) || double.IsInfinity(n))
            return string.Empty;

        if (Math.Abs(n) < 1e15 && Math.Abs(n - Math.Truncate(n)) < 1e-9)
            return ((long)Math.Truncate(n)).ToString(CultureInfo.InvariantCulture);

        return n.ToString("G15", CultureInfo.InvariantCulture);
    }

    private static int ParseQty(IXLCell cell)
    {
        try
        {
            if (cell.TryGetValue<int>(out var asInt))
                return Math.Max(0, asInt);

            if (cell.DataType == XLDataType.Number)
                return Math.Max(0, (int)Math.Round(cell.GetDouble(), MidpointRounding.AwayFromZero));
        }
        catch
        {
            // parsed from text below
        }

        var text = CellText(cell);
        if (string.IsNullOrWhiteSpace(text))
            return 0;

        text = text.Replace(" ", string.Empty).Replace('\u00A0', '\0').Replace("\0", string.Empty);
        if (int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var i)
            || int.TryParse(text, NumberStyles.Integer, CultureInfo.CurrentCulture, out i))
            return Math.Max(0, i);

        if (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var d)
            || double.TryParse(text, NumberStyles.Float, CultureInfo.GetCultureInfo("fr-FR"), out d)
            || double.TryParse(text, NumberStyles.Float, CultureInfo.CurrentCulture, out d))
            return Math.Max(0, (int)Math.Round(d, MidpointRounding.AwayFromZero));

        return 0;
    }

    private sealed class ColumnMap
    {
        public bool HasHeaderRow { get; set; }
        public int CodeEmplacement { get; set; } = 1;
        public int DesignationEmplacement { get; set; } = 2;
        public int CodeUS { get; set; } = 3;
        public int DesignationUS { get; set; } = 4;
        public int QteTheorique { get; set; } = 5;
        public int CodeAgent { get; set; } = 6;
        public int Client { get; set; } = 7;
    }
}
