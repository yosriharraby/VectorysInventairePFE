using MediatR;
using VectorysInventaire.Application.DTOs;
using VectorysInventaire.Application.Features.Scans.Commands.EnregistrerScan;

namespace VectorysInventaire.Application.Features.Scans.Commands.EnregistrerScansBatch;

public class EnregistrerScansBatchCommandHandler
    : IRequestHandler<EnregistrerScansBatchCommand, EnregistrerScansBatchResultDto>
{
    private readonly IMediator _mediator;

    public EnregistrerScansBatchCommandHandler(IMediator mediator) => _mediator = mediator;

    public async Task<EnregistrerScansBatchResultDto> Handle(
        EnregistrerScansBatchCommand request,
        CancellationToken cancellationToken)
    {
        var items = new List<ScanBatchItemResultDto>();
        var imported = 0;
        var duplicates = 0;
        var errors = 0;

        foreach (var scan in request.Scans)
        {
            var offlineCommand = scan with { IsOfflineSync = true };
            try
            {
                var result = await _mediator.Send(offlineCommand, cancellationToken);
                var isDuplicate = result.ScanId == 0 &&
                    !string.IsNullOrWhiteSpace(scan.IdLocalFlutter);

                if (isDuplicate)
                {
                    duplicates++;
                    items.Add(new ScanBatchItemResultDto
                    {
                        IdLocalFlutter = scan.IdLocalFlutter,
                        Success = true,
                        IsDuplicate = true,
                        Result = result
                    });
                }
                else
                {
                    imported++;
                    items.Add(new ScanBatchItemResultDto
                    {
                        IdLocalFlutter = scan.IdLocalFlutter,
                        Success = true,
                        Result = result
                    });
                }
            }
            catch (Exception ex)
            {
                errors++;
                items.Add(new ScanBatchItemResultDto
                {
                    IdLocalFlutter = scan.IdLocalFlutter,
                    Success = false,
                    ErrorMessage = ex.Message
                });
            }
        }

        return new EnregistrerScansBatchResultDto
        {
            TotalRecu = request.Scans.Count,
            TotalImporte = imported,
            TotalDoublons = duplicates,
            TotalErreurs = errors,
            Items = items
        };
    }
}
