using MediatR;
using VectorysInventaire.Application.DTOs;
using VectorysInventaire.Application.Interfaces.Services;

namespace VectorysInventaire.Application.Features.Ecarts.Commands.AppliquerBrancheB;

public class AppliquerBrancheBCommandHandler : IRequestHandler<AppliquerBrancheBCommand, AppliquerBrancheBResultDto>
{
    private readonly IStockImportService _stockImportService;
    private readonly ICurrentUserService _currentUser;

    public AppliquerBrancheBCommandHandler(
        IStockImportService stockImportService,
        ICurrentUserService currentUser)
    {
        _stockImportService = stockImportService;
        _currentUser = currentUser;
    }

    public Task<AppliquerBrancheBResultDto> Handle(
        AppliquerBrancheBCommand request,
        CancellationToken cancellationToken)
    {
        var mode = BrancheBModes.Normalize(request.Mode);
        var superviseurId = request.SuperviseurId ?? _currentUser.UserId;
        return _stockImportService.ImportBrancheBAsync(
            request.InventaireId,
            request.FileStream,
            mode,
            request.FichierNom,
            request.AssignationMode,
            request.AgentCode,
            superviseurId,
            cancellationToken);
    }
}
