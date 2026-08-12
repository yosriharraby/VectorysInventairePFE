using FluentValidation;

namespace VectorysInventaire.Application.Features.Inventaires.Commands.CloturerInventaire;

public class CloturerInventaireCommandValidator : AbstractValidator<CloturerInventaireCommand>
{
    public CloturerInventaireCommandValidator()
    {
        RuleFor(x => x.InventaireId).GreaterThan(0);
    }
}
