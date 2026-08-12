using MediatR;
using VectorysInventaire.Application.DTOs;
using VectorysInventaire.Domain.Interfaces.Repositories;

namespace VectorysInventaire.Application.Features.Inventaires.Queries.GetInventaires;

public class GetInventairesQueryHandler : IRequestHandler<GetInventairesQuery, PagedResultDto<InventaireDto>>
{
    private readonly IInventaireRepository _inventaireRepository;

    public GetInventairesQueryHandler(IInventaireRepository inventaireRepository)
    {
        _inventaireRepository = inventaireRepository;
    }

    public async Task<PagedResultDto<InventaireDto>> Handle(GetInventairesQuery request, CancellationToken cancellationToken)
    {
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize is < 1 or > 100 ? 20 : request.PageSize;

        var (items, total) = await _inventaireRepository.GetPagedAsync(page, pageSize, cancellationToken);

        return new PagedResultDto<InventaireDto>
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = total,
            Items = items.Select(i => new InventaireDto
            {
                Id = i.Id,
                Reference = i.Reference,
                Intitule = i.Intitule,
                Zone = i.Zone,
                DateDebut = i.DateDebut,
                DateFin = i.DateFin,
                Statut = i.Statut.ToString(),
                SuperviseurId = i.SuperviseurId,
                CreatedAt = i.CreatedAt
            }).ToList()
        };
    }
}
