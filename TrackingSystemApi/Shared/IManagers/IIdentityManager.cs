using System.Security.Claims;
using TrackingSystem.Api.Shared.Dto.Identity;
using TrackingSystem.Api.Shared.SharedModels;

namespace TrackingSystem.Api.Shared.IManagers
{
    public interface IIdentityManager
    {
        Task<ClaimsIdentity> CreateIdentity(CreateIdentityCommand query, CancellationToken cancellationToken);

        Task<ResponseModel<RefreshTokenResponseDTO>> RefreshToken(RefreshTokenCommand command, CancellationToken cancellationToken);
    }
}
