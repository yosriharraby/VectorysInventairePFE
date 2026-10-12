using MediatR;
using VectorysInventaire.Application.DTOs;

namespace VectorysInventaire.Application.Features.Emplacements.Queries.GetMesEmplacements;

public record GetMesEmplacementsQuery : IRequest<IReadOnlyList<MesEmplacementDto>>;

public record MesEmplacementDto(
    int Id,
    string Code,
    string Designation,
    string Statut,
    string CodeBarres,
    int QteAttendue,
    int QteScannee,
    int InventaireId,
    string InventaireReference,
    string InventaireIntitule,
    string InventaireZone,
    IReadOnlyList<StockLigneProgressDto> LignesStock,
    string PassageType = "C1",
    int? RecomptageId = null,
    string? RecomptageLibelle = null,
    string? RecomptageStatut = null);
