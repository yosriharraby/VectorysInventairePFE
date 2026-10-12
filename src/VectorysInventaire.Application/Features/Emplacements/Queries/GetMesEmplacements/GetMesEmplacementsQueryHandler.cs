using MediatR;
using VectorysInventaire.Application.DTOs;
using VectorysInventaire.Application.Interfaces.Services;
using VectorysInventaire.Domain.Entities;
using VectorysInventaire.Domain.Enums;
using VectorysInventaire.Domain.Interfaces.Repositories;

namespace VectorysInventaire.Application.Features.Emplacements.Queries.GetMesEmplacements;

public class GetMesEmplacementsQueryHandler : IRequestHandler<GetMesEmplacementsQuery, IReadOnlyList<MesEmplacementDto>>
{
    private readonly IEmplacementRepository _emplacementRepository;
    private readonly ICurrentUserService _currentUser;

    public GetMesEmplacementsQueryHandler(
        IEmplacementRepository emplacementRepository,
        ICurrentUserService currentUser)
    {
        _emplacementRepository = emplacementRepository;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<MesEmplacementDto>> Handle(
        GetMesEmplacementsQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
            throw new UnauthorizedAccessException("Utilisateur non authentifié.");

        var list = await _emplacementRepository.GetByAgentIdWithProgressAsync(
            _currentUser.UserId.Value, cancellationToken);

        return list
            .Where(e => e.Passage?.Inventaire?.Statut == StatutInventaireEnum.EN_COURS)
            .OrderBy(e => e.Passage!.InventaireId)
            .ThenBy(e => e.RecomptageId == null ? 0 : 1)
            .ThenBy(e => e.RecomptageId)
            .ThenBy(e => e.Code)
            .Select(e =>
            {
                var scanCounts = e.Scans
                    .GroupBy(s => s.CodeBarresScan)
                    .ToDictionary(g => g.Key, g => g.Count(), StringComparer.OrdinalIgnoreCase);

                var lignes = e.StocksTheoriques
                    .OrderBy(s => s.CodeUS)
                    .Select(s => new StockLigneProgressDto(
                        s.CodeUS,
                        s.DesignationUS,
                        s.QteTheorique,
                        scanCounts.GetValueOrDefault(s.CodeUS, 0),
                        s.Client))
                    .ToList();

                return new MesEmplacementDto(
                    e.Id,
                    e.Code,
                    e.Designation,
                    e.Statut.ToString(),
                    e.CodeBarres,
                    e.StocksTheoriques.Sum(s => s.QteTheorique),
                    e.Scans.Count,
                    e.Passage!.InventaireId,
                    e.Passage.Inventaire?.Reference ?? string.Empty,
                    e.Passage.Inventaire?.Intitule ?? string.Empty,
                    e.Passage.Inventaire?.Zone ?? string.Empty,
                    lignes,
                    e.Passage.Type.ToString(),
                    e.RecomptageId,
                    e.Recomptage is null
                        ? null
                        : RecomptageStatuts.Libelle(e.Recomptage.Mode, e.Recomptage.FichierNom),
                    e.Recomptage?.Statut);
            })
            .ToList();
    }
}
