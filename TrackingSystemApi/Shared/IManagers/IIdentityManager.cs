using System.Security.Claims;
using TrackingSystem.Api.Shared.Dto.Identity;
using TrackingSystem.Api.Shared.SharedModels;

namespace TrackingSystem.Api.Shared.IManagers
{
    public interface IIdentityManager
    {
        ClaimsIdentity CreateIdentity(CreateIdentityDto query, CancellationToken cancellationToken = default);

        ResponseModel<RefreshTokenResponseDTO> RefreshToken(RefreshTokenDto command, CancellationToken cancellationToken = default);
    }
}
