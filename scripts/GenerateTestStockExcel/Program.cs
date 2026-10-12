using ClosedXML.Excel;

// Agents AG001 / AG002 confirmés dans VectorysDB (YOSRI\SQLEXPRESS).
// Codes TEST-* pour ne pas entrer en conflit avec A-01-xx déjà importés.
var rows = new (string CodeEmplacement, string DesignationEmplacement, string CodeUS, string DesignationUS, int QteTheorique, string CodeAgent, string Client)[]
{
    ("TEST-A01", "Zone test A - Rack 01", "US-T001", "Carton test 12L", 2, "AG001", "Sagemcom"),
    ("TEST-A01", "Zone test A - Rack 01", "US-T002", "Bouteille test 1L", 1, "AG001", "Valeo"),
    ("TEST-A02", "Zone test A - Rack 02", "US-T003", "Palette test mixte", 2, "AG001", "Forvia"),
    ("TEST-B01", "Zone test B - Rack 01", "US-T101", "Produit test A", 1, "AG002", "Lear"),
    ("TEST-B01", "Zone test B - Rack 01", "US-T102", "Produit test B", 2, "AG002", "Leoni"),
    ("TEST-B02", "Zone test B - Rack 02", "US-T201", "Produit test C", 1, "AG002", "Dräxlmaier"),
};

var outputDir = args.Length > 0 ? args[0] : Path.Combine("..", "..");
Directory.CreateDirectory(outputDir);
var outputPath = Path.Combine(outputDir, "test-import-stock.xlsx");

using var workbook = new XLWorkbook();
var ws = workbook.Worksheets.Add("Stock");

ws.Cell(1, 1).Value = "CodeEmplacement";
ws.Cell(1, 2).Value = "DesignationEmplacement";
ws.Cell(1, 3).Value = "CodeUS";
ws.Cell(1, 4).Value = "DesignationUS";
ws.Cell(1, 5).Value = "QteTheorique";
ws.Cell(1, 6).Value = "CodeAgent";
ws.Cell(1, 7).Value = "Client";

var header = ws.Range(1, 1, 1, 7);
header.Style.Font.Bold = true;
header.Style.Fill.BackgroundColor = XLColor.FromHtml("#EEF2FF");

for (var i = 0; i < rows.Length; i++)
{
    var r = rows[i];
    var row = i + 2;
    ws.Cell(row, 1).Value = r.CodeEmplacement;
    ws.Cell(row, 2).Value = r.DesignationEmplacement;
    ws.Cell(row, 3).Value = r.CodeUS;
    ws.Cell(row, 4).Value = r.DesignationUS;
    ws.Cell(row, 5).Value = r.QteTheorique;
    ws.Cell(row, 6).Value = r.CodeAgent;
    ws.Cell(row, 7).Value = r.Client;
}

ws.Columns().AdjustToContents();
workbook.SaveAs(outputPath);

Console.WriteLine($"Fichier généré : {Path.GetFullPath(outputPath)}");
Console.WriteLine($"Lignes : {rows.Length} | Emplacements : 4 | Agents : AG001 (2), AG002 (2)");
