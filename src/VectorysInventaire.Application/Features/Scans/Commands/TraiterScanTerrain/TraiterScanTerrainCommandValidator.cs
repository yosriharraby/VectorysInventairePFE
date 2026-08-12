using FluentValidation;

namespace VectorysInventaire.Application.Features.Scans.Commands.TraiterScanTerrain;

public class TraiterScanTerrainCommandValidator : AbstractValidator<TraiterScanTerrainCommand>
{
    public TraiterScanTerrainCommandValidator()
    {
        RuleFor(x => x.CodeBarresScan)
            .NotEmpty()
            .WithMessage("Le code scanné est obligatoire.");
    }
}
