using FluentValidation;

namespace VectorysInventaire.Application.Features.Ecarts.Commands.AppliquerBrancheB;

public class AppliquerBrancheBCommandValidator : AbstractValidator<AppliquerBrancheBCommand>
{
    public AppliquerBrancheBCommandValidator()
    {
        RuleFor(x => x.InventaireId).GreaterThan(0);
        RuleFor(x => x.Mode)
            .NotEmpty()
            .Must(BrancheBModes.IsValid)
            .WithMessage("Mode Branche B invalide. Utilisez GLOBAL ou CIBLE.");
        RuleFor(x => x.FileStream)
            .NotNull()
            .Must(s => s.CanRead)
            .WithMessage("Fichier Excel requis.");
        RuleFor(x => x.AssignationMode)
            .Must(AssignationRecomptageModes.IsValid)
            .When(x => !string.IsNullOrWhiteSpace(x.AssignationMode))
            .WithMessage("Mode d'assignation invalide.");
        RuleFor(x => x.AgentCode)
            .NotEmpty()
            .When(x => string.Equals(
                AssignationRecomptageModes.Normalize(x.AssignationMode, x.Mode),
                AssignationRecomptageModes.NouveauAgent,
                StringComparison.OrdinalIgnoreCase))
            .WithMessage("Code agent requis pour l'assignation « nouveau agent ».");
    }
}
