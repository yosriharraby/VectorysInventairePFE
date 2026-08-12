using MediatR;
using VectorysInventaire.Application.DTOs;

namespace VectorysInventaire.Application.Features.Passages.Commands.ImportStock;

public record ImportStockCommand(
    int PassageId,
    Stream FileStream,
    IReadOnlyList<string>? DefaultCodeAgents = null) : IRequest<ImportStockResultDto>;
