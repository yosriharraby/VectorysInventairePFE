using MediatR;

namespace VectorysInventaire.Application.Features.Inventaires.Queries.ExportInventaire;

public record ExportInventaireQuery(int InventaireId) : IRequest<(byte[] Content, string FileName)>;
