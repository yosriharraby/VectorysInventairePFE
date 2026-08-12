using FluentValidation;

namespace VectorysInventaire.Application.Features.Scans.Commands.EnregistrerScansBatch;

public class EnregistrerScansBatchCommandValidator : AbstractValidator<EnregistrerScansBatchCommand>
{
    public EnregistrerScansBatchCommandValidator()
    {
        RuleFor(x => x.Scans).NotEmpty().WithMessage("La liste des scans ne peut pas être vide.");
        RuleFor(x => x.Scans.Count).LessThanOrEqualTo(500)
            .WithMessage("Maximum 500 scans par lot.");
    }
}
