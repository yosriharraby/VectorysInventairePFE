using FluentValidation;
using MediatR;
using VectorysInventaire.Application.DTOs;
using VectorysInventaire.Application.Interfaces.Services;
using VectorysInventaire.Domain.Entities;
using VectorysInventaire.Domain.Enums;
using VectorysInventaire.Domain.Interfaces.Repositories;

namespace VectorysInventaire.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
{
    private readonly IUtilisateurRepository _utilisateurRepository;
    private readonly IJwtService _jwtService;
    private readonly IPasswordHasher _passwordHasher;

    public LoginCommandHandler(
        IUtilisateurRepository utilisateurRepository,
        IJwtService jwtService,
        IPasswordHasher passwordHasher)
    {
        _utilisateurRepository = utilisateurRepository;
        _jwtService = jwtService;
        _passwordHasher = passwordHasher;
    }

    public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        Utilisateur? user = null;

        if (!string.IsNullOrWhiteSpace(request.CodePersonnel))
        {
            user = await _utilisateurRepository.GetByCodePersonnelAsync(
                request.CodePersonnel.Trim(),
                cancellationToken);
            if (user is null || user.Role != RoleEnum.AGENT)
                throw new UnauthorizedAccessException("Identifiants invalides.");

            if (!_passwordHasher.Verify(request.CodePersonnel.Trim(), user.PasswordHash))
                throw new UnauthorizedAccessException("Identifiants invalides.");
        }
        else if (!string.IsNullOrWhiteSpace(request.Email) && !string.IsNullOrWhiteSpace(request.Password))
        {
            user = await _utilisateurRepository.GetByEmailAsync(request.Email.Trim(), cancellationToken);
            if (user is null || (user.Role != RoleEnum.SUPERVISEUR && user.Role != RoleEnum.ADMIN))
                throw new UnauthorizedAccessException("Identifiants invalides.");

            if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Identifiants invalides.");
        }
        else
        {
            throw new UnauthorizedAccessException("Identifiants invalides.");
        }

        if (!user.Actif)
            throw new UnauthorizedAccessException("Compte désactivé.");

        var token = _jwtService.GenerateToken(user);
        return new AuthResponseDto
        {
            Token = token,
            Role = user.Role.ToString(),
            Nom = $"{user.Prenom} {user.Nom}".Trim(),
            UserId = user.Id
        };
    }
}
