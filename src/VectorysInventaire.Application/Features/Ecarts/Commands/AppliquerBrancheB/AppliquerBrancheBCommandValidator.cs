using FluentValidation;

namespace VectorysInventaire.Application.Features.Ecarts.Commands.AppliquerBrancheB;

public class AppliquerBrancheBCommandValidator : AbstractValidator<AppliquerBrancheBCommand>
{
    public AppliquerBrancheBCommandValidator()
    {
        RuleFor(x => x.InventaireId).GreaterThan(0);
    }
}
