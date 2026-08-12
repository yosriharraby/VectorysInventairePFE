using MediatR;
using VectorysInventaire.Application.DTOs;

namespace VectorysInventaire.Application.Features.Auth.Commands.Login;

public record LoginCommand(string? Email, string? Password, string? CodePersonnel)
    : IRequest<AuthResponseDto>;
