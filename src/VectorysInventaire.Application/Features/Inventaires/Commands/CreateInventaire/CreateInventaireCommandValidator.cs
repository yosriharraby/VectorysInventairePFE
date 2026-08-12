using FluentValidation;

namespace VectorysInventaire.Application.Features.Inventaires.Commands.CreateInventaire;

public class CreateInventaireCommandValidator : AbstractValidator<CreateInventaireCommand>
{
    public CreateInventaireCommandValidator()
    {
        RuleFor(x => x.Intitule).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Zone).NotEmpty().MaximumLength(100);
        RuleFor(x => x.DateDebut).NotEmpty();
        RuleFor(x => x.DateFin)
            .GreaterThanOrEqualTo(x => x.DateDebut)
            .When(x => x.DateFin.HasValue);
    }
}
