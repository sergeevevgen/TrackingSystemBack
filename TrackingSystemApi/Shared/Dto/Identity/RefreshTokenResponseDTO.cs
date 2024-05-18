using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackingSystem.Api.Shared.Dto.Identity
{
    public class RefreshTokenResponseDTO
    {
        public string AccessToken { get; init; } = null!;

        public string? Login { get; init; }

        public string Id { get; init; } = null!;

        public string Role { get; init; } = null!;
    }
}
