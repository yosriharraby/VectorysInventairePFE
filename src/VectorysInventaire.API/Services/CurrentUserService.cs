using System.Security.Claims;
using VectorysInventaire.Application.Interfaces.Services;

namespace VectorysInventaire.API.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true;

    public int? UserId
    {
        get
        {
            var id = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name)
                     ?? _httpContextAccessor.HttpContext?.User?.FindFirstValue("sub");
            return int.TryParse(id, out var userId) ? userId : null;
        }
    }

    public string? Nom =>
        _httpContextAccessor.HttpContext?.User?.FindFirstValue("nom")
        ?? _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name);

    public string? Role =>
        _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role);
}
