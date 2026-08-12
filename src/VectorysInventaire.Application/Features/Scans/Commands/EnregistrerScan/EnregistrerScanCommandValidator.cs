using FluentValidation;

namespace VectorysInventaire.Application.Features.Scans.Commands.EnregistrerScan;

public class EnregistrerScanCommandValidator : AbstractValidator<EnregistrerScanCommand>
{
    public EnregistrerScanCommandValidator()
    {
        RuleFor(x => x.CodeBarresScan).NotEmpty().MaximumLength(128);
        RuleFor(x => x.EmplacementId).GreaterThan(0);
        RuleFor(x => x.DateHeureScan).NotEmpty();
        RuleFor(x => x.IdLocalFlutter).MaximumLength(64).When(x => x.IdLocalFlutter != null);
    }
}
