using MediatR;
using VectorysInventaire.Application.DTOs;
using VectorysInventaire.Domain.Enums;
using VectorysInventaire.Domain.Interfaces.Repositories;

namespace VectorysInventaire.Application.Features.Inventaires.Queries.GetSuiviTempsReel;

public class GetSuiviTempsReelQueryHandler : IRequestHandler<GetSuiviTempsReelQuery, SuiviTempsReelDto>
{
    private readonly IInventaireRepository _inventaireRepository;
    private readonly IPassageRepository _passageRepository;
    private readonly IEmplacementRepository _emplacementRepository;

    public GetSuiviTempsReelQueryHandler(
        IInventaireRepository inventaireRepository,
        IPassageRepository passageRepository,
        IEmplacementRepository emplacementRepository)
    {
        _inventaireRepository = inventaireRepository;
        _passageRepository = passageRepository;
        _emplacementRepository = emplacementRepository;
    }

    public async Task<SuiviTempsReelDto> Handle(GetSuiviTempsReelQuery request, CancellationToken cancellationToken)
    {
        var inventaire = await _inventaireRepository.GetByIdAsync(request.InventaireId, cancellationToken)
            ?? throw new KeyNotFoundException("Inventaire introuvable.");

        var passage = await _passageRepository.GetByInventaireAndTypeAsync(
            request.InventaireId, TypePassageEnum.C1, cancellationToken)
            ?? throw new KeyNotFoundException("Passage C1 introuvable pour cet inventaire.");

        var emplacements = await _emplacementRepository.GetByPassageIdWithProgressAsync(passage.Id, cancellationToken);

        var agents = emplacements
            .Where(e => e.AgentAssigneId.HasValue)
            .GroupBy(e => e.AgentAssigneId!.Value)
            .Select(g =>
            {
                var agent = g.First().AgentAssigne;
                var emplacementSuivis = g.Select(MapEmplacement).ToList();
                var actif = emplacementSuivis.Any(e =>
                    e.Statut is "EN_COURS" or "EN_ATTENTE" && e.QteScannee > 0)
                    || emplacementSuivis.Any(e => e.Statut == "EN_COURS");

                return new AgentSuiviDto
                {
                    AgentId = g.Key,
                    Nom = agent != null ? $"{agent.Prenom} {agent.Nom}".Trim() : $"Agent {g.Key}",
                    CodePersonnel = agent?.CodePersonnel,
                    EstActif = actif,
                    EmplacementsTermines = emplacementSuivis.Count(e => e.Statut == "TERMINE"),
                    EmplacementsTotal = emplacementSuivis.Count,
                    Emplacements = emplacementSuivis
                };
            })
            .OrderBy(a => a.Nom)
            .ToList();

        return new SuiviTempsReelDto
        {
            InventaireId = inventaire.Id,
            Intitule = inventaire.Intitule,
            PassageC1Id = passage.Id,
            Agents = agents
        };
    }

    private static EmplacementSuiviDto MapEmplacement(Domain.Entities.Emplacement e)
    {
        var qteAttendue = e.StocksTheoriques.Sum(s => s.QteTheorique);
        var qteScannee = e.Scans.Count;
        var dernier = e.Scans.OrderByDescending(s => s.DateHeureScan).FirstOrDefault();

        return new EmplacementSuiviDto
        {
            Id = e.Id,
            Code = e.Code,
            Designation = e.Designation,
            Statut = e.Statut.ToString(),
            QteScannee = qteScannee,
            QteAttendue = qteAttendue,
            DernierScan = dernier?.DateHeureScan
        };
    }
}
