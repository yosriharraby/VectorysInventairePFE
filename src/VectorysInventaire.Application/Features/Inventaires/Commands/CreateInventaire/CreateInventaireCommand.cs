using MediatR;
using VectorysInventaire.Application.DTOs;

namespace VectorysInventaire.Application.Features.Inventaires.Commands.CreateInventaire;

public record CreateInventaireCommand(string Intitule, string Zone, DateTime DateDebut, DateTime? DateFin)
    : IRequest<CreateInventaireResultDto>;
