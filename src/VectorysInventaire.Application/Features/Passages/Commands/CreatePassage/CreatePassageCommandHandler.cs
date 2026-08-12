using MediatR;
using VectorysInventaire.Application.DTOs;
using VectorysInventaire.Domain.Entities;
using VectorysInventaire.Domain.Enums;
using VectorysInventaire.Domain.Interfaces.Repositories;

namespace VectorysInventaire.Application.Features.Passages.Commands.CreatePassage;

public class CreatePassageCommandHandler : IRequestHandler<CreatePassageCommand, PassageDto>
{
    private readonly IInventaireRepository _inventaireRepository;
    private readonly IPassageRepository _passageRepository;

    public CreatePassageCommandHandler(
        IInventaireRepository inventaireRepository,
        IPassageRepository passageRepository)
    {
        _inventaireRepository = inventaireRepository;
        _passageRepository = passageRepository;
    }

    public async Task<PassageDto> Handle(CreatePassageCommand request, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<TypePassageEnum>(request.Type, true, out var type))
            throw new ArgumentException("Type de passage invalide (C1 ou C2).");

        var inventaire = await _inventaireRepository.GetByIdAsync(request.InventaireId, cancellationToken)
            ?? throw new KeyNotFoundException("Inventaire introuvable.");

        var existing = await _passageRepository.GetByInventaireAndTypeAsync(inventaire.Id, type, cancellationToken);
        if (existing is not null)
            throw new InvalidOperationException($"Un passage {type} existe déjà pour cet inventaire.");

        var passage = new Passage
        {
            InventaireId = inventaire.Id,
            Type = type,
            Numero = type == TypePassageEnum.C1 ? 1 : 2,
            Statut = "EN_PREPARATION",
            CreatedAt = DateTime.UtcNow
        };

        await _passageRepository.AddAsync(passage, cancellationToken);
        await _inventaireRepository.SaveChangesAsync(cancellationToken);

        return new PassageDto
        {
            Id = passage.Id,
            InventaireId = passage.InventaireId,
            Type = passage.Type.ToString(),
            Numero = passage.Numero,
            Statut = passage.Statut
        };
    }
}
