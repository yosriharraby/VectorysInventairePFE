using MediatR;
using VectorysInventaire.Application.DTOs;
using VectorysInventaire.Application.Interfaces.Services;
using VectorysInventaire.Domain.Entities;
using VectorysInventaire.Domain.Enums;
using VectorysInventaire.Domain.Interfaces.Repositories;

namespace VectorysInventaire.Application.Features.Inventaires.Commands.CreateInventaire;

public class CreateInventaireCommandHandler : IRequestHandler<CreateInventaireCommand, CreateInventaireResultDto>
{
    private readonly IInventaireRepository _inventaireRepository;
    private readonly IPassageRepository _passageRepository;
    private readonly ICurrentUserService _currentUser;

    public CreateInventaireCommandHandler(
        IInventaireRepository inventaireRepository,
        IPassageRepository passageRepository,
        ICurrentUserService currentUser)
    {
        _inventaireRepository = inventaireRepository;
        _passageRepository = passageRepository;
        _currentUser = currentUser;
    }

    public async Task<CreateInventaireResultDto> Handle(CreateInventaireCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
            throw new UnauthorizedAccessException("Utilisateur non authentifié.");

        var year = DateTime.UtcNow.Year;
        var maxSequence = await _inventaireRepository.GetMaxReferenceSequenceForYearAsync(year, cancellationToken);
        var reference = $"INV-{year}-{(maxSequence + 1):D3}";

        var inventaire = new Inventaire
        {
            Reference = reference,
            Intitule = request.Intitule.Trim(),
            Zone = request.Zone.Trim(),
            DateDebut = request.DateDebut,
            DateFin = request.DateFin,
            Statut = StatutInventaireEnum.EN_PREPARATION,
            SuperviseurId = _currentUser.UserId.Value,
            CreatedAt = DateTime.UtcNow
        };

        await _inventaireRepository.AddAsync(inventaire, cancellationToken);
        await _inventaireRepository.SaveChangesAsync(cancellationToken);

        var passage = new Passage
        {
            InventaireId = inventaire.Id,
            Type = TypePassageEnum.C1,
            Numero = 1,
            Statut = "EN_PREPARATION",
            CreatedAt = DateTime.UtcNow
        };

        await _passageRepository.AddAsync(passage, cancellationToken);
        await _inventaireRepository.SaveChangesAsync(cancellationToken);

        return new CreateInventaireResultDto
        {
            Inventaire = MapInventaire(inventaire),
            PassageC1Id = passage.Id
        };
    }

    private static InventaireDto MapInventaire(Inventaire i) => new()
    {
        Id = i.Id,
        Reference = i.Reference,
        Intitule = i.Intitule,
        Zone = i.Zone,
        DateDebut = i.DateDebut,
        DateFin = i.DateFin,
        Statut = i.Statut.ToString(),
        SuperviseurId = i.SuperviseurId,
        CreatedAt = i.CreatedAt
    };
}
