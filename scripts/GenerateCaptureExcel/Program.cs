using ClosedXML.Excel;

var rows = new (string CodeEmplacement, string DesignationEmplacement, string CodeUS, string DesignationUS, int QteTheorique, string CodeAgent, string Client)[]
{
    ("SWG-A01", "Swagger A - Rack 01", "US-S001", "Carton swagger", 2, "AG001", "Sagemcom"),
    ("SWG-A01", "Swagger A - Rack 01", "US-S002", "Bouteille swagger", 1, "AG001", "Valeo"),
    ("SWG-B02", "Swagger B - Rack 02", "US-S201", "Produit swagger C", 1, "AG002", "Forvia"),
};

var outputPath = Path.Combine("c:\\Users\\yosri\\Desktop\\VectorysInventaire\\scripts", "swagger-open-stock.xlsx");
using var workbook = new XLWorkbook();
var ws = workbook.Worksheets.Add("Stock");
string[] headers = ["CodeEmplacement", "DesignationEmplacement", "CodeUS", "DesignationUS", "QteTheorique", "CodeAgent", "Client"];
for (var c = 0; c < headers.Length; c++)
    ws.Cell(1, c + 1).Value = headers[c];

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
Console.WriteLine(outputPath);
