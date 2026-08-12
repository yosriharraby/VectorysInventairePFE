using MediatR;
using VectorysInventaire.Application.DTOs;
using VectorysInventaire.Domain.Enums;
using VectorysInventaire.Domain.Interfaces.Repositories;

namespace VectorysInventaire.Application.Features.Inventaires.Commands.CloturerInventaire;

public class CloturerInventaireCommandHandler : IRequestHandler<CloturerInventaireCommand, CloturerInventaireResultDto>
{
    private readonly IInventaireRepository _inventaireRepository;

    public CloturerInventaireCommandHandler(IInventaireRepository inventaireRepository)
    {
        _inventaireRepository = inventaireRepository;
    }

    public async Task<CloturerInventaireResultDto> Handle(CloturerInventaireCommand request, CancellationToken cancellationToken)
    {
        var inventaire = await _inventaireRepository.GetByIdForUpdateAsync(request.InventaireId, cancellationToken)
            ?? throw new KeyNotFoundException("Inventaire introuvable.");

        if (inventaire.Statut == StatutInventaireEnum.CLOTURE)
            throw new InvalidOperationException("Cet inventaire est déjà clôturé.");

        if (inventaire.Statut == StatutInventaireEnum.EN_PREPARATION)
            throw new InvalidOperationException("Impossible de clôturer un inventaire encore en préparation.");

        inventaire.Statut = StatutInventaireEnum.CLOTURE;
        inventaire.DateFin = DateTime.UtcNow;

        await _inventaireRepository.SaveChangesAsync(cancellationToken);

        return new CloturerInventaireResultDto
        {
            Inventaire = new InventaireDto
            {
                Id = inventaire.Id,
                Reference = inventaire.Reference,
                Intitule = inventaire.Intitule,
                Zone = inventaire.Zone,
                DateDebut = inventaire.DateDebut,
                DateFin = inventaire.DateFin,
                Statut = inventaire.Statut.ToString(),
                SuperviseurId = inventaire.SuperviseurId,
                CreatedAt = inventaire.CreatedAt
            },
            Message = $"Inventaire {inventaire.Reference} clôturé."
        };
    }
}
