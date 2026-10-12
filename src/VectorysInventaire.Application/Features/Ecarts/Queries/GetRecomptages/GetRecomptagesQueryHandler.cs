using MediatR;
using VectorysInventaire.Application.DTOs;
using VectorysInventaire.Application.Interfaces.Services;
using VectorysInventaire.Domain.Entities;
using VectorysInventaire.Domain.Enums;
using VectorysInventaire.Domain.Interfaces.Repositories;

namespace VectorysInventaire.Application.Features.Ecarts.Queries.GetRecomptages;

public class GetRecomptagesQueryHandler : IRequestHandler<GetRecomptagesQuery, RecomptagesListDto>
{
    private readonly IInventaireRepository _inventaireRepository;
    private readonly IRecomptageRepository _recomptageRepository;
    private readonly IEcartRepository _ecartRepository;
    private readonly IRecomptageProgressService _progressService;

    public GetRecomptagesQueryHandler(
        IInventaireRepository inventaireRepository,
        IRecomptageRepository recomptageRepository,
        IEcartRepository ecartRepository,
        IRecomptageProgressService progressService)
    {
        _inventaireRepository = inventaireRepository;
        _recomptageRepository = recomptageRepository;
        _ecartRepository = ecartRepository;
        _progressService = progressService;
    }

    public async Task<RecomptagesListDto> Handle(GetRecomptagesQuery request, CancellationToken cancellationToken)
    {
        var inventaire = await _inventaireRepository.GetByIdAsync(request.InventaireId, cancellationToken)
            ?? throw new KeyNotFoundException("Inventaire introuvable.");

        var recomptages = await _recomptageRepository.GetByInventaireIdAsync(request.InventaireId, cancellationToken);
        foreach (var recomptage in recomptages)
            await _progressService.RefreshAsync(recomptage.Id, cancellationToken);

        recomptages = await _recomptageRepository.GetByInventaireIdAsync(request.InventaireId, cancellationToken);
        var allEcarts = await _ecartRepository.GetByInventaireIdAsync(request.InventaireId, cancellationToken);

        var items = recomptages.Select(r =>
        {
            var codes = r.Emplacements
                .Select(e => e.Code)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(c => c)
                .ToList();
            var restants = allEcarts.Count(e =>
                e.Statut == StatutEcartEnum.DETECTE
                && codes.Contains(e.CodeEmplacement, StringComparer.OrdinalIgnoreCase));

            return new RecomptageDto
            {
                Id = r.Id,
                InventaireId = r.InventaireId,
                PassageC2Id = r.PassageC2Id,
                Mode = r.Mode,
                AssignationMode = r.AssignationMode,
                FichierNom = r.FichierNom,
                SuperviseurNom = r.Superviseur is null
                    ? "—"
                    : $"{r.Superviseur.Prenom} {r.Superviseur.Nom}".Trim(),
                EmplacementsCrees = r.EmplacementsCrees,
                EcartsTraites = r.EcartsTraites,
                EcartsRestants = restants,
                Statut = r.Statut,
                CreatedAt = r.CreatedAt,
                EmplacementCodes = codes,
                Agents = r.Agents.Select(a => new RecomptageAgentDto
                {
                    AgentId = a.AgentId,
                    CodePersonnel = a.Agent?.CodePersonnel ?? "—",
                    NomComplet = a.Agent is null ? "—" : $"{a.Agent.Prenom} {a.Agent.Nom}".Trim(),
                    EmplacementsAssignes = a.EmplacementsAssignes
                }).ToList()
            };
        }).ToList();

        return new RecomptagesListDto
        {
            InventaireId = inventaire.Id,
            InventaireReference = inventaire.Reference,
            TotalRecomptages = items.Count,
            Items = items
        };
    }
}
