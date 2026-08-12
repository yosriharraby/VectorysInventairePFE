using FluentValidation;

namespace VectorysInventaire.Application.Features.Ecarts.Commands.AppliquerBrancheA;

public class AppliquerBrancheACommandValidator : AbstractValidator<AppliquerBrancheACommand>
{
    public AppliquerBrancheACommandValidator()
    {
        RuleFor(x => x.InventaireId).GreaterThan(0);
    }
}
