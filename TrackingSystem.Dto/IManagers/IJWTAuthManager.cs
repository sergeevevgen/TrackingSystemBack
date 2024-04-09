using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using TrackingSystem.Shared.Enums;

namespace TrackingSystem.Shared.IManagers
{
    public interface IJWTAuthManager
    {
        string GenerateToken(IEnumerable<Claim> claims, EJwtTokenType type);

        SymmetricSecurityKey GetSymmetricSecurityKey(EJwtTokenType type);
    }
}
