using MediatR;
using VectorysInventaire.Application.DTOs;
using VectorysInventaire.Domain.Enums;
using VectorysInventaire.Domain.Interfaces.Repositories;

namespace VectorysInventaire.Application.Features.Ecarts.Queries.GetEcartsCount;

public class GetEcartsCountQueryHandler : IRequestHandler<GetEcartsCountQuery, EcartsCountDto>
{
    private readonly IEcartRepository _ecartRepository;

    public GetEcartsCountQueryHandler(IEcartRepository ecartRepository) => _ecartRepository = ecartRepository;

    public async Task<EcartsCountDto> Handle(GetEcartsCountQuery request, CancellationToken cancellationToken)
    {
        var total = await _ecartRepository.CountByStatutAsync(StatutEcartEnum.DETECTE, cancellationToken);
        return new EcartsCountDto { TotalDetectes = total };
    }
}
