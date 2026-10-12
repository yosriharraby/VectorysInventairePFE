using MediatR;
using VectorysInventaire.Application.DTOs;
using VectorysInventaire.Domain.Entities;
using VectorysInventaire.Domain.Enums;
using VectorysInventaire.Domain.Interfaces.Repositories;

namespace VectorysInventaire.Application.Features.Ecarts.Queries.GetEcarts;

public class GetEcartsQueryHandler : IRequestHandler<GetEcartsQuery, EcartsListDto>
{
    private readonly IInventaireRepository _inventaireRepository;
    private readonly IEcartRepository _ecartRepository;
    private readonly IRecomptageRepository _recomptageRepository;

    public GetEcartsQueryHandler(
        IInventaireRepository inventaireRepository,
        IEcartRepository ecartRepository,
        IRecomptageRepository recomptageRepository)
    {
        _inventaireRepository = inventaireRepository;
        _ecartRepository = ecartRepository;
        _recomptageRepository = recomptageRepository;
    }

    public async Task<EcartsListDto> Handle(GetEcartsQuery request, CancellationToken cancellationToken)
    {
        var inventaire = await _inventaireRepository.GetByIdAsync(request.InventaireId, cancellationToken)
            ?? throw new KeyNotFoundException("Inventaire introuvable.");

        var ecarts = await _ecartRepository.GetByInventaireIdAsync(request.InventaireId, cancellationToken);
        var recomptages = await _recomptageRepository.GetByInventaireIdAsync(request.InventaireId, cancellationToken);
        var dejaTraites = EcartBusinessKey.Traites(ecarts);
        var pending = ecarts
            .Where(e => e.Statut == StatutEcartEnum.DETECTE && !dejaTraites.Contains(EcartBusinessKey.Of(e)))
            .ToList();
        var pendingItems = pending.Select(Map).ToList();
        var allItems = ecarts.Select(Map).ToList();

        return new EcartsListDto
        {
            InventaireId = inventaire.Id,
            InventaireReference = inventaire.Reference,
            TotalDetectes = pendingItems.Count,
            TotalTraites = allItems.Count(e =>
                e.Statut == StatutEcartEnum.TRAITE_BRANCHE_A.ToString()
                || e.Statut == StatutEcartEnum.TRAITE_BRANCHE_B.ToString()),
            TotalManquants = pendingItems.Count(e => e.Type == TypeEcartEnum.MANQUANT.ToString()),
            TotalSurplus = pendingItems.Count(e => e.Type == TypeEcartEnum.SURPLUS.ToString()),
            TousRecomptagesCorriges = recomptages.Count > 0
                && recomptages.All(r => r.Statut == RecomptageStatuts.Corrige),
            Items = pendingItems
        };
    }

    internal static EcartDto Map(Domain.Entities.Ecart e) => new()
    {
        Id = e.Id,
        InventaireId = e.InventaireId,
        EmplacementId = e.EmplacementId,
        CodeEmplacement = e.CodeEmplacement,
        DesignationEmplacement = e.DesignationEmplacement,
        CodeUS = e.CodeUS,
        DesignationUS = e.DesignationUS,
        Client = e.Client,
        QteTheorique = e.QteTheorique,
        QteScannee = e.QteScannee,
        Delta = e.Delta,
        Type = e.Type.ToString(),
        Statut = e.Statut.ToString(),
        DetecteAt = e.DetecteAt,
        TraiteAt = e.TraiteAt
    };
}
