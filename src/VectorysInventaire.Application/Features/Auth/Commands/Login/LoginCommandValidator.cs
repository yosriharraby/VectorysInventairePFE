using System.ComponentModel.DataAnnotations;
using FluentValidation;

namespace VectorysInventaire.Application.Features.Auth.Commands.Login;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x)
            .Custom((c, ctx) =>
            {
                var agent = !string.IsNullOrWhiteSpace(c.CodePersonnel);
                var sup = !string.IsNullOrWhiteSpace(c.Email) && !string.IsNullOrWhiteSpace(c.Password);
                if (agent == sup)
                {
                    ctx.AddFailure("Requête",
                        "Fournissez le code personnel (agent) ou email et mot de passe (superviseur), pas les deux.");
                    return;
                }

                if (sup && !new EmailAddressAttribute().IsValid(c.Email))
                    ctx.AddFailure(nameof(c.Email), "Email invalide.");
            });
    }
}
