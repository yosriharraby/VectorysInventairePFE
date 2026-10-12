namespace VectorysInventaire.Application.Interfaces.Services;

public interface IRecomptageProgressService
{
    Task RefreshByEmplacementIdAsync(int emplacementId, CancellationToken cancellationToken = default);
    Task RefreshAsync(int recomptageId, CancellationToken cancellationToken = default);
}
