using MediatR;
using VectorysInventaire.Application.DTOs;
using VectorysInventaire.Domain.Entities;
using VectorysInventaire.Domain.Enums;
using VectorysInventaire.Domain.Interfaces.Repositories;

namespace VectorysInventaire.Application.Features.Ecarts.Commands.AppliquerBrancheA;

public class AppliquerBrancheACommandHandler : IRequestHandler<AppliquerBrancheACommand, AppliquerBrancheAResultDto>
{
    private readonly IInventaireRepository _inventaireRepository;
    private readonly IPassageRepository _passageRepository;
    private readonly IEmplacementRepository _emplacementRepository;
    private readonly IStockTheoriqueRepository _stockRepository;
    private readonly IEcartRepository _ecartRepository;

    public AppliquerBrancheACommandHandler(
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

    public async Task<AppliquerBrancheAResultDto> Handle(AppliquerBrancheACommand request, CancellationToken cancellationToken)
    {
        _ = await _inventaireRepository.GetByIdAsync(request.InventaireId, cancellationToken)
            ?? throw new KeyNotFoundException("Inventaire introuvable.");

        var passageC1 = await _passageRepository.GetByInventaireAndTypeAsync(
            request.InventaireId, TypePassageEnum.C1, cancellationToken)
            ?? throw new KeyNotFoundException("Passage C1 introuvable.");

        IReadOnlyList<Ecart> ecarts;
        if (request.EcartIds.Count > 0)
        {
            ecarts = await _ecartRepository.GetByIdsAsync(request.EcartIds, cancellationToken);
            if (ecarts.Any(e => e.InventaireId != request.InventaireId))
                throw new InvalidOperationException("Certains écarts n'appartiennent pas à cet inventaire.");
        }
        else
        {
            var all = await _ecartRepository.GetByInventaireIdAsync(request.InventaireId, cancellationToken);
            var ids = all.Where(e => e.Statut == StatutEcartEnum.DETECTE).Select(e => e.Id).ToList();
            ecarts = await _ecartRepository.GetByIdsAsync(ids, cancellationToken);
        }

        var aTraiter = ecarts.Where(e => e.Statut == StatutEcartEnum.DETECTE).ToList();
        if (aTraiter.Count == 0)
            throw new InvalidOperationException("Aucun écart en attente de traitement Branche A.");

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

        var emplacementIds = aTraiter.Select(e => e.EmplacementId).Distinct().ToList();
        var emplacementsCrees = 0;
        var now = DateTime.UtcNow;

        foreach (var emplacementId in emplacementIds)
        {
            var c1 = await _emplacementRepository.GetByIdAsync(emplacementId, cancellationToken)
                ?? throw new KeyNotFoundException($"Emplacement {emplacementId} introuvable.");

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
            ecart.Statut = StatutEcartEnum.TRAITE_BRANCHE_A;
            ecart.TraiteAt = now;
        }

        await _ecartRepository.SaveChangesAsync(cancellationToken);

        return new AppliquerBrancheAResultDto
        {
            InventaireId = request.InventaireId,
            PassageC2Id = passageC2.Id,
            EcartsTraites = aTraiter.Count,
            EmplacementsCreesC2 = emplacementsCrees,
            Message = $"Branche A appliquée : {aTraiter.Count} écart(s) traité(s), passage C2 #{passageC2.Id} préparé."
        };
    }
}
