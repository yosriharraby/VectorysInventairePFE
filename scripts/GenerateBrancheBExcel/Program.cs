using ClosedXML.Excel;

var scriptsDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
var repoScripts = Path.Combine(scriptsDir, "test-excel-v2");

var downloads = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
    "Downloads", "Vectorys-Test-Excel-v2");

Directory.CreateDirectory(repoScripts);
Directory.CreateDirectory(downloads);

string[] headers =
[
    "CodeEmplacement",
    "DesignationEmplacement",
    "CodeUS",
    "DesignationUS",
    "QteTheorique",
    "CodeAgent",
    "Client"
];

var allFiles = new List<(string Name, Row[] Rows, string Description)>();
allFiles.AddRange(BuildSprintFiles("S7", "7", "S701", "S702", "S703", "S711", "S712", "S721", "S999"));
allFiles.AddRange(BuildSprintFiles("S8", "8", "S801", "S802", "S803", "S811", "S812", "S821", "S899"));

Console.WriteLine("=== Fichiers Excel de test (v2) ===\n");

foreach (var (name, rows, description) in allFiles)
{
    var repoPath = Path.Combine(repoScripts, name);
    var dlPath = Path.Combine(downloads, name);
    WriteExcel(repoPath, rows, headers);
    WriteExcel(dlPath, rows, headers);
    Console.WriteLine($"  {name}");
    Console.WriteLine($"    → {description}");
}

Console.WriteLine($"\nDossiers :");
Console.WriteLine($"  {repoScripts}");
Console.WriteLine($"  {downloads}");
Console.WriteLine("\nScénario recommandé (S8 — inventaire neuf) :");
Console.WriteLine("  1. Créer inventaire → importer s8-c1-initial.xlsx");
Console.WriteLine("  2. Comptages agents (S8-A01 : 1× US-S801 seulement) → Détecter écarts (2 MANQUANT)");
Console.WriteLine("  3. Sprint 5 : branche-a | Sprint 6 : branche-b GLOBAL ou CIBLÉ");
Console.WriteLine("  4. Test erreur : s8-b-cible-erreur.xlsx doit être rejeté (400)");

static (string Name, Row[] Rows, string Description)[] BuildSprintFiles(
    string prefix,
    string sprintNum,
    string usA01a,
    string usA01b,
    string usA02,
    string usB01a,
    string usB01b,
    string usB02,
    string usErreur)
{
    var tag = prefix.ToLowerInvariant();

    var c1Initial = new Row[]
    {
        new($"{prefix}-A01", $"Sprint{sprintNum} A - Rack 01", $"US-{usA01a}", $"Carton {prefix} 12L", 3, "AG001", "Sagemcom"),
        new($"{prefix}-A01", $"Sprint{sprintNum} A - Rack 01", $"US-{usA01b}", $"Bouteille {prefix} 1L", 2, "AG001", "Valeo"),
        new($"{prefix}-A02", $"Sprint{sprintNum} A - Rack 02", $"US-{usA02}", $"Palette {prefix} mixte", 2, "AG001", "Forvia"),
        new($"{prefix}-B01", $"Sprint{sprintNum} B - Rack 01", $"US-{usB01a}", $"Produit {prefix} B1", 2, "AG002", "Lear"),
        new($"{prefix}-B01", $"Sprint{sprintNum} B - Rack 01", $"US-{usB01b}", $"Produit {prefix} B2", 1, "AG002", "Leoni"),
        new($"{prefix}-B02", $"Sprint{sprintNum} B - Rack 02", $"US-{usB02}", $"Produit {prefix} C", 1, "AG002", "Dräxlmaier"),
    };

    var bGlobal = new Row[]
    {
        c1Initial[0], c1Initial[1], c1Initial[2], c1Initial[3],
    };

    var bCibleA01 = c1Initial.Where(r => r.Emp == $"{prefix}-A01").ToArray();
    var bCibleA02 = c1Initial.Where(r => r.Emp == $"{prefix}-A02").ToArray();
    var bCibleB01 = c1Initial.Where(r => r.Emp == $"{prefix}-B01").ToArray();

    var bCibleErreur = new Row[]
    {
        c1Initial[0],
        new($"{prefix}-Z99", "Emplacement invalide", $"US-{usErreur}", "Produit fictif", 1, "AG001", "Forvia"),
    };

    return
    [
        ($"{tag}-c1-initial.xlsx", c1Initial,
            $"Import C1 — 4 emplacements ({prefix}-A01, A02, B01, B02)"),
        ($"{tag}-b-global.xlsx", bGlobal,
            $"Branche B GLOBAL — 3 emplacements (A01, A02, B01)"),
        ($"{tag}-b-cible-a01.xlsx", bCibleA01,
            $"Branche B CIBLÉ — {prefix}-A01 seulement (2 US)"),
        ($"{tag}-b-cible-a02.xlsx", bCibleA02,
            $"Branche B CIBLÉ — {prefix}-A02 seulement (1 US)"),
        ($"{tag}-b-cible-b01.xlsx", bCibleB01,
            $"Branche B CIBLÉ — {prefix}-B01 seulement (2 US)"),
        ($"{tag}-b-cible-erreur.xlsx", bCibleErreur,
            $"Test erreur CIBLÉ — contient {prefix}-Z99 hors C1"),
    ];
}

static void WriteExcel(string path, Row[] rows, string[] headers)
{
    using var workbook = new XLWorkbook();
    var ws = workbook.Worksheets.Add("Stock");
    for (var c = 0; c < headers.Length; c++)
        ws.Cell(1, c + 1).Value = headers[c];

    for (var i = 0; i < rows.Length; i++)
    {
        var r = rows[i];
        var row = i + 2;
        ws.Cell(row, 1).Value = r.Emp;
        ws.Cell(row, 2).Value = r.EmpDes;
        ws.Cell(row, 3).Value = r.Us;
        ws.Cell(row, 4).Value = r.UsDes;
        ws.Cell(row, 5).Value = r.Qte;
        ws.Cell(row, 6).Value = r.Agent;
        ws.Cell(row, 7).Value = r.Client;
    }

    ws.Columns().AdjustToContents();
    workbook.SaveAs(path);
}

readonly record struct Row(string Emp, string EmpDes, string Us, string UsDes, int Qte, string Agent, string Client);
