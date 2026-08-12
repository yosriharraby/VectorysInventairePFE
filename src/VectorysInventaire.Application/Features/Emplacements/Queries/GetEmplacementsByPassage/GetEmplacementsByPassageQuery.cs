using MediatR;
using VectorysInventaire.Application.DTOs;

namespace VectorysInventaire.Application.Features.Emplacements.Queries.GetEmplacementsByPassage;

public record GetEmplacementsByPassageQuery(int PassageId) : IRequest<IReadOnlyList<EmplacementDto>>;
