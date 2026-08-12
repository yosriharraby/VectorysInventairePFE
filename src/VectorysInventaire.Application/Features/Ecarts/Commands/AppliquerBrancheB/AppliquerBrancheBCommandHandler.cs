using MediatR;
using VectorysInventaire.Application.DTOs;
using VectorysInventaire.Domain.Entities;
using VectorysInventaire.Domain.Enums;
using VectorysInventaire.Domain.Interfaces.Repositories;

namespace VectorysInventaire.Application.Features.Ecarts.Commands.AppliquerBrancheB;

/// <summary>
/// Branche B : recomptage global — clone tous les emplacements C1 vers C2
/// et marque tous les écarts DETECTE comme TRAITE_BRANCHE_B.
/// </summary>
public class AppliquerBrancheBCommandHandler : IRequestHandler<AppliquerBrancheBCommand, AppliquerBrancheBResultDto>
{
    private readonly IInventaireRepository _inventaireRepository;
    private readonly IPassageRepository _passageRepository;
    private readonly IEmplacementRepository _emplacementRepository;
    private readonly IStockTheoriqueRepository _stockRepository;
    private readonly IEcartRepository _ecartRepository;

    public AppliquerBrancheBCommandHandler(
        IInventaireRepository inventaireRepository,
        IPassageRepository passageRepository,
        IEmplacementRepository emplacementRepository,
        IStockTheoriqueRepository stockRepository,
        IEcartRepository ecartRepository)
    {
        _inventaireRepository = inventaireRepository;
        _passageRepository = passageRepository;
        _emplacementRepository = emplacementRepository;
        _stockRepository = stockRepository;
        _ecartRepository = ecartRepository;
    }

    public async Task<AppliquerBrancheBResultDto> Handle(AppliquerBrancheBCommand request, CancellationToken cancellationToken)
    {
        _ = await _inventaireRepository.GetByIdAsync(request.InventaireId, cancellationToken)
            ?? throw new KeyNotFoundException("Inventaire introuvable.");

        var passageC1 = await _passageRepository.GetByInventaireAndTypeAsync(
            request.InventaireId, TypePassageEnum.C1, cancellationToken)
            ?? throw new KeyNotFoundException("Passage C1 introuvable.");

        var allEcarts = await _ecartRepository.GetByInventaireIdAsync(request.InventaireId, cancellationToken);
        var detecteIds = allEcarts
            .Where(e => e.Statut == StatutEcartEnum.DETECTE)
            .Select(e => e.Id)
            .ToList();
        var aTraiter = detecteIds.Count > 0
            ? await _ecartRepository.GetByIdsAsync(detecteIds, cancellationToken)
            : Array.Empty<Ecart>();

        var passageC2 = await _passageRepository.GetByInventaireAndTypeAsync(
            request.InventaireId, TypePassageEnum.C2, cancellationToken);

        if (passageC2 is null)
        {
            passageC2 = new Passage
            {
                InventaireId = request.InventaireId,
                Type = TypePassageEnum.C2,
                Numero = 2,
                Statut = "EN_PREPARATION",
                CreatedAt = DateTime.UtcNow
            };
            await _passageRepository.AddAsync(passageC2, cancellationToken);
            await _emplacementRepository.SaveChangesAsync(cancellationToken);
        }

        var emplacementsC1 = await _emplacementRepository.GetByPassageIdAsync(passageC1.Id, cancellationToken);
        var emplacementsCrees = 0;
        var now = DateTime.UtcNow;

        foreach (var c1 in emplacementsC1)
        {
            var existingC2 = await _emplacementRepository.GetByPassageIdAndCodeAsync(
                passageC2.Id, c1.Code, cancellationToken);
            if (existingC2 is not null)
                continue;

            var c2 = new Emplacement
            {
                PassageId = passageC2.Id,
                Code = c1.Code,
                Designation = c1.Designation,
                CodeBarres = c1.CodeBarres,
                Statut = StatutEmplacementEnum.EN_ATTENTE,
                AgentAssigneId = c1.AgentAssigneId
            };

            var stocks = await _stockRepository.GetByEmplacementIdAsync(c1.Id, cancellationToken);
            foreach (var stock in stocks)
            {
                c2.StocksTheoriques.Add(new StockTheorique
                {
                    CodeUS = stock.CodeUS,
                    DesignationUS = stock.DesignationUS,
                    QteTheorique = stock.QteTheorique,
                    CreatedAt = now
                });
            }

            await _emplacementRepository.AddRangeAsync(new[] { c2 }, cancellationToken);
            emplacementsCrees++;
        }

        foreach (var ecart in aTraiter)
        {
            ecart.Statut = StatutEcartEnum.TRAITE_BRANCHE_B;
            ecart.TraiteAt = now;
        }

        await _ecartRepository.SaveChangesAsync(cancellationToken);

        return new AppliquerBrancheBResultDto
        {
            InventaireId = request.InventaireId,
            PassageC2Id = passageC2.Id,
            EcartsTraites = aTraiter.Count,
            EmplacementsCreesC2 = emplacementsCrees,
            Message = $"Branche B appliquée : recomptage global — {emplacementsCrees} emplacement(s) cloné(s) vers C2 #{passageC2.Id}, {aTraiter.Count} écart(s) traité(s)."
        };
    }
}
