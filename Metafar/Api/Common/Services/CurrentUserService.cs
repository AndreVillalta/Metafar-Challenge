using Api.Common.Interfaces;
using System.IdentityModel.Tokens.Jwt;

namespace Api.Common.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor? _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;

        // Probablemente se está inicializando la aplicación.
        if (_httpContextAccessor is null || _httpContextAccessor.HttpContext is null)
        {
            User = new CurrentUser(0, Guid.Empty.ToString(), string.Empty, false);

            return;
        }

        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext!.User!.Identity!.IsAuthenticated == false)
        {
            User = new CurrentUser(0, Guid.Empty.ToString(), string.Empty, false);

            return;
        }

        var accountNumber = httpContext.User.Claims
                            .FirstOrDefault(q => q.Type == JwtRegisteredClaimNames.Jti)!
                            .Value;

        var userId = httpContext.User.Claims
                    .FirstOrDefault(q => q.Type == JwtRegisteredClaimNames.Sid)!
                    .Value;

        var userName = httpContext.User.Identity.Name!;

        User = new CurrentUser(Convert.ToInt32(userId), accountNumber, userName, true);
    }

    public CurrentUser User { get; }
}
