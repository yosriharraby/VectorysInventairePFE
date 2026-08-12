using MediatR;
using VectorysInventaire.Application.DTOs;
using VectorysInventaire.Domain.Enums;
using VectorysInventaire.Domain.Interfaces.Repositories;

namespace VectorysInventaire.Application.Features.Ecarts.Queries.GetEcarts;

public class GetEcartsQueryHandler : IRequestHandler<GetEcartsQuery, EcartsListDto>
{
    private readonly IInventaireRepository _inventaireRepository;
    private readonly IEcartRepository _ecartRepository;

    public GetEcartsQueryHandler(IInventaireRepository inventaireRepository, IEcartRepository ecartRepository)
    {
        _inventaireRepository = inventaireRepository;
        _ecartRepository = ecartRepository;
    }

    public async Task<EcartsListDto> Handle(GetEcartsQuery request, CancellationToken cancellationToken)
    {
        var inventaire = await _inventaireRepository.GetByIdAsync(request.InventaireId, cancellationToken)
            ?? throw new KeyNotFoundException("Inventaire introuvable.");

        var ecarts = await _ecartRepository.GetByInventaireIdAsync(request.InventaireId, cancellationToken);
        var items = ecarts.Select(Map).ToList();

        return new EcartsListDto
        {
            InventaireId = inventaire.Id,
            InventaireReference = inventaire.Reference,
            TotalDetectes = items.Count(e => e.Statut == StatutEcartEnum.DETECTE.ToString()),
            TotalTraites = items.Count(e =>
                e.Statut == StatutEcartEnum.TRAITE_BRANCHE_A.ToString()
                || e.Statut == StatutEcartEnum.TRAITE_BRANCHE_B.ToString()),
            TotalManquants = items.Count(e => e.Type == TypeEcartEnum.MANQUANT.ToString()),
            TotalSurplus = items.Count(e => e.Type == TypeEcartEnum.SURPLUS.ToString()),
            Items = items
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
        QteTheorique = e.QteTheorique,
        QteScannee = e.QteScannee,
        Delta = e.Delta,
        Type = e.Type.ToString(),
        Statut = e.Statut.ToString(),
        DetecteAt = e.DetecteAt,
        TraiteAt = e.TraiteAt
    };
}
