using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using TrackingSystem.Api.Shared.Enums;

namespace TrackingSystem.Api.Shared.IManagers
{
    public interface IJWTAuthManager
    {
        string GenerateToken(IEnumerable<Claim> claims, EJwtTokenType type);

        SymmetricSecurityKey GetSymmetricSecurityKey(EJwtTokenType type);
    }
}
