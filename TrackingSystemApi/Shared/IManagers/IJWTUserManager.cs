using System.Security.Claims;
using TrackingSystem.Api.Shared.Dto.User;

namespace TrackingSystem.Api.Shared.IManagers
{
    public interface IJWTUserManager
    {
        Task<UserResponseData> GetUserByIdentity(ClaimsIdentity identity);

        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
