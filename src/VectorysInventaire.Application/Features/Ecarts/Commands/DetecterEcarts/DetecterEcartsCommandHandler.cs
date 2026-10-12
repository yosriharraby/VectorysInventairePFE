using MediatR;
using VectorysInventaire.Application.DTOs;
using VectorysInventaire.Application.Features.Ecarts;
using VectorysInventaire.Application.Features.Ecarts.Queries.GetEcarts;
using VectorysInventaire.Domain.Entities;
using VectorysInventaire.Domain.Enums;
using VectorysInventaire.Domain.Interfaces.Repositories;

namespace VectorysInventaire.Application.Features.Ecarts.Commands.DetecterEcarts;

public class DetecterEcartsCommandHandler : IRequestHandler<DetecterEcartsCommand, DetecterEcartsResultDto>
{
    private readonly IInventaireRepository _inventaireRepository;
    private readonly IPassageRepository _passageRepository;
    private readonly IEmplacementRepository _emplacementRepository;
    private readonly IEcartRepository _ecartRepository;

    public DetecterEcartsCommandHandler(
        IInventaireRepository inventaireRepository,
        IPassageRepository passageRepository,
        IEmplacementRepository emplacementRepository,
        IEcartRepository ecartRepository)
    {
        _inventaireRepository = inventaireRepository;
        _passageRepository = passageRepository;
        _emplacementRepository = emplacementRepository;
        _ecartRepository = ecartRepository;
    }

    public async Task<DetecterEcartsResultDto> Handle(DetecterEcartsCommand request, CancellationToken cancellationToken)
    {
        _ = await _inventaireRepository.GetByIdAsync(request.InventaireId, cancellationToken)
            ?? throw new KeyNotFoundException("Inventaire introuvable.");

        var passage = await _passageRepository.GetByInventaireAndTypeAsync(
            request.InventaireId, TypePassageEnum.C1, cancellationToken)
            ?? throw new KeyNotFoundException("Passage C1 introuvable pour cet inventaire.");

        var emplacements = await _emplacementRepository.GetByPassageIdWithProgressAsync(passage.Id, cancellationToken);
        var existants = await _ecartRepository.GetByInventaireIdAsync(request.InventaireId, cancellationToken);
        var dejaTraites = EcartBusinessKey.Traites(existants);
        var now = DateTime.UtcNow;
        var ecarts = new List<Ecart>();
        var nonRouverts = 0;

        foreach (var emp in emplacements.Where(e => e.Statut == StatutEmplacementEnum.TERMINE))
        {
            var scansByCode = emp.Scans
                .GroupBy(s => s.CodeBarresScan.Trim(), StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.Count(), StringComparer.OrdinalIgnoreCase);

            foreach (var stock in emp.StocksTheoriques)
            {
                scansByCode.TryGetValue(stock.CodeUS.Trim(), out var qteScannee);
                if (qteScannee == stock.QteTheorique)
                    continue;

                if (dejaTraites.Contains(EcartBusinessKey.Of(emp.Code, stock.CodeUS)))
                {
                    nonRouverts++;
                    continue;
                }

                var delta = qteScannee - stock.QteTheorique;
                ecarts.Add(new Ecart
                {
                    InventaireId = request.InventaireId,
                    PassageId = passage.Id,
                    EmplacementId = emp.Id,
                    CodeEmplacement = emp.Code,
                    DesignationEmplacement = emp.Designation,
                    CodeUS = stock.CodeUS,
                    DesignationUS = stock.DesignationUS,
                    Client = stock.Client,
                    QteTheorique = stock.QteTheorique,
                    QteScannee = qteScannee,
                    Delta = delta,
                    Type = delta < 0 ? TypeEcartEnum.MANQUANT : TypeEcartEnum.SURPLUS,
                    Statut = StatutEcartEnum.DETECTE,
                    DetecteAt = now
                });
            }

            var stockCodes = emp.StocksTheoriques
                .Select(s => s.CodeUS.Trim())
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            foreach (var (code, qte) in scansByCode)
            {
                if (stockCodes.Contains(code))
                    continue;

                if (dejaTraites.Contains(EcartBusinessKey.Of(emp.Code, code)))
                {
                    nonRouverts++;
                    continue;
                }

                ecarts.Add(new Ecart
                {
                    InventaireId = request.InventaireId,
                    PassageId = passage.Id,
                    EmplacementId = emp.Id,
                    CodeEmplacement = emp.Code,
                    DesignationEmplacement = emp.Designation,
                    CodeUS = code,
                    DesignationUS = "US non référencée",
                    QteTheorique = 0,
                    QteScannee = qte,
                    Delta = qte,
                    Type = TypeEcartEnum.SURPLUS,
                    Statut = StatutEcartEnum.DETECTE,
                    DetecteAt = now
                });
            }
        }

        await _ecartRepository.DeleteDetectesByInventaireIdAsync(request.InventaireId, cancellationToken);
        if (ecarts.Count > 0)
            await _ecartRepository.AddRangeAsync(ecarts, cancellationToken);
        await _ecartRepository.SaveChangesAsync(cancellationToken);

        var items = ecarts.Select(GetEcartsQueryHandler.Map).ToList();
        return new DetecterEcartsResultDto
        {
            InventaireId = request.InventaireId,
            EcartsDetectes = items.Count,
            EcartsDejaTraitesNonRouverts = nonRouverts,
            Message = BuildMessage(items.Count, nonRouverts),
            Items = items
        };
    }

    private static string BuildMessage(int detectes, int nonRouverts)
    {
        if (detectes == 0 && nonRouverts == 0)
            return "Aucun écart détecté.";
        if (detectes == 0)
            return $"Aucun nouvel écart. {nonRouverts} écart(s) déjà traités (A/B), non rouverts.";
        if (nonRouverts == 0)
            return $"{detectes} écart(s) détecté(s).";
        return $"{detectes} écart(s) détecté(s). {nonRouverts} écart(s) déjà traités (A/B), non rouverts.";
    }
}
