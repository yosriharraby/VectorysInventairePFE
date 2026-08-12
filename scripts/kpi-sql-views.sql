-- Vues SQL Server optionnelles pour KPI (production / scale)
-- Exécuter manuellement sur VectorysDB si agrégats nightly souhaités

IF OBJECT_ID(N'dbo.vw_KpiLignesControlees', N'V') IS NOT NULL
    DROP VIEW dbo.vw_KpiLignesControlees;
GO

CREATE VIEW dbo.vw_KpiLignesControlees AS
SELECT
    i.Id AS InventaireId,
    i.Zone,
    i.DateDebut,
    COUNT(st.Id) AS LignesStockControlees
FROM Inventaires i
INNER JOIN Passages p ON p.InventaireId = i.Id AND p.Type = 1 -- C1
INNER JOIN Emplacements e ON e.PassageId = p.Id AND e.Statut = 2 -- TERMINE
INNER JOIN StockTheorique st ON st.EmplacementId = e.Id
GROUP BY i.Id, i.Zone, i.DateDebut;
GO

IF OBJECT_ID(N'dbo.vw_KpiEcartsDaily', N'V') IS NOT NULL
    DROP VIEW dbo.vw_KpiEcartsDaily;
GO

CREATE VIEW dbo.vw_KpiEcartsDaily AS
SELECT
    CAST(e.DetecteAt AS date) AS Jour,
    e.InventaireId,
    i.Zone,
    COUNT(*) AS NbEcarts,
    SUM(ABS(e.Delta)) AS DeltaUnites
FROM Ecart e
INNER JOIN Inventaires i ON i.Id = e.InventaireId
GROUP BY CAST(e.DetecteAt AS date), e.InventaireId, i.Zone;
GO

IF OBJECT_ID(N'dbo.vw_KpiScansDaily', N'V') IS NOT NULL
    DROP VIEW dbo.vw_KpiScansDaily;
GO

CREATE VIEW dbo.vw_KpiScansDaily AS
SELECT
    CAST(s.DateHeureScan AS date) AS Jour,
    p.InventaireId,
    COUNT(*) AS NbScans,
    COUNT(DISTINCT s.EmplacementId) AS EmplacementsTouches
FROM Scans s
INNER JOIN Emplacements e ON e.Id = s.EmplacementId
INNER JOIN Passages p ON p.Id = e.PassageId
GROUP BY CAST(s.DateHeureScan AS date), p.InventaireId;
GO

-- Index complémentaires (déjà migrés via EF Sprint7_KpiIndexes)
-- IX_Ecart_InventaireId_DetecteAt, IX_Ecart_Statut_DetecteAt, IX_Scans_DateHeureScan
