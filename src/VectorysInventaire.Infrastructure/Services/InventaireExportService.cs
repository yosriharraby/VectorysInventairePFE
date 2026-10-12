using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using VectorysInventaire.Application.Interfaces.Services;
using VectorysInventaire.Infrastructure.Persistence;

namespace VectorysInventaire.Infrastructure.Services;

public class InventaireExportService : IInventaireExportService
{
    private readonly VectorysDbContext _db;

    public InventaireExportService(VectorysDbContext db) => _db = db;

    public async Task<byte[]> ExportInventaireAsync(int inventaireId, CancellationToken cancellationToken = default)
    {
        var inventaire = await _db.Inventaires.AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == inventaireId, cancellationToken)
            ?? throw new KeyNotFoundException("Inventaire introuvable.");

        var passages = await _db.Passages.AsNoTracking()
            .Where(p => p.InventaireId == inventaireId)
            .OrderBy(p => p.Numero)
            .ToListAsync(cancellationToken);

        var passageIds = passages.Select(p => p.Id).ToList();

        var emplacements = await _db.Emplacements.AsNoTracking()
            .Include(e => e.AgentAssigne)
            .Include(e => e.StocksTheoriques)
            .Include(e => e.Scans)
            .Where(e => passageIds.Contains(e.PassageId))
            .OrderBy(e => e.PassageId)
            .ThenBy(e => e.Code)
            .ToListAsync(cancellationToken);

        var ecarts = await _db.Ecarts.AsNoTracking()
            .Where(e => e.InventaireId == inventaireId)
            .OrderBy(e => e.CodeEmplacement)
            .ThenBy(e => e.CodeUS)
            .ToListAsync(cancellationToken);

        using var workbook = new XLWorkbook();

        var resume = workbook.Worksheets.Add("Resume");
        resume.Cell(1, 1).Value = "Reference";
        resume.Cell(1, 2).Value = inventaire.Reference;
        resume.Cell(2, 1).Value = "Intitule";
        resume.Cell(2, 2).Value = inventaire.Intitule;
        resume.Cell(3, 1).Value = "Zone";
        resume.Cell(3, 2).Value = inventaire.Zone;
        resume.Cell(4, 1).Value = "Statut";
        resume.Cell(4, 2).Value = inventaire.Statut.ToString();
        resume.Cell(5, 1).Value = "DateDebut";
        resume.Cell(5, 2).Value = inventaire.DateDebut;
        resume.Cell(6, 1).Value = "DateFin";
        resume.Cell(6, 2).Value = inventaire.DateFin;
        resume.Cell(7, 1).Value = "Emplacements";
        resume.Cell(7, 2).Value = emplacements.Count;
        resume.Cell(8, 1).Value = "Ecarts";
        resume.Cell(8, 2).Value = ecarts.Count;
        resume.Columns().AdjustToContents();

        var sheetEmp = workbook.Worksheets.Add("Emplacements");
        string[] empHeaders =
        [
            "Passage", "Code", "Designation", "Statut", "Agent",
            "QteTheorique", "QteScannee", "Delta"
        ];
        for (var c = 0; c < empHeaders.Length; c++)
            sheetEmp.Cell(1, c + 1).Value = empHeaders[c];

        var row = 2;
        var passageById = passages.ToDictionary(p => p.Id);
        foreach (var e in emplacements)
        {
            var passageType = passageById.TryGetValue(e.PassageId, out var p) ? p.Type.ToString() : "?";
            var qteTheo = e.StocksTheoriques.Sum(s => s.QteTheorique);
            var qteScan = e.Scans.Count;
            sheetEmp.Cell(row, 1).Value = passageType;
            sheetEmp.Cell(row, 2).Value = e.Code;
            sheetEmp.Cell(row, 3).Value = e.Designation;
            sheetEmp.Cell(row, 4).Value = e.Statut.ToString();
            sheetEmp.Cell(row, 5).Value = e.AgentAssigne?.CodePersonnel ?? "";
            sheetEmp.Cell(row, 6).Value = qteTheo;
            sheetEmp.Cell(row, 7).Value = qteScan;
            sheetEmp.Cell(row, 8).Value = qteScan - qteTheo;
            row++;
        }
        sheetEmp.Columns().AdjustToContents();

        var sheetEcarts = workbook.Worksheets.Add("Ecarts");
        string[] ecartHeaders =
        [
            "CodeEmplacement", "CodeUS", "Client", "QteTheorique", "QteScannee", "Delta", "Type", "Statut"
        ];
        for (var c = 0; c < ecartHeaders.Length; c++)
            sheetEcarts.Cell(1, c + 1).Value = ecartHeaders[c];

        row = 2;
        foreach (var e in ecarts)
        {
            sheetEcarts.Cell(row, 1).Value = e.CodeEmplacement;
            sheetEcarts.Cell(row, 2).Value = e.CodeUS;
            sheetEcarts.Cell(row, 3).Value = e.Client ?? string.Empty;
            sheetEcarts.Cell(row, 4).Value = e.QteTheorique;
            sheetEcarts.Cell(row, 5).Value = e.QteScannee;
            sheetEcarts.Cell(row, 6).Value = e.Delta;
            sheetEcarts.Cell(row, 7).Value = e.Type.ToString();
            sheetEcarts.Cell(row, 8).Value = e.Statut.ToString();
            row++;
        }
        sheetEcarts.Columns().AdjustToContents();

        var sheetScans = workbook.Worksheets.Add("Scans");
        string[] scanHeaders = ["Emplacement", "CodeBarres", "AgentId", "DateHeureScan", "EstOffline"];
        for (var c = 0; c < scanHeaders.Length; c++)
            sheetScans.Cell(1, c + 1).Value = scanHeaders[c];

        row = 2;
        foreach (var e in emplacements)
        {
            foreach (var s in e.Scans.OrderBy(x => x.DateHeureScan))
            {
                sheetScans.Cell(row, 1).Value = e.Code;
                sheetScans.Cell(row, 2).Value = s.CodeBarresScan;
                sheetScans.Cell(row, 3).Value = s.AgentId;
                sheetScans.Cell(row, 4).Value = s.DateHeureScan;
                sheetScans.Cell(row, 5).Value = s.EstOffline;
                row++;
            }
        }
        sheetScans.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
