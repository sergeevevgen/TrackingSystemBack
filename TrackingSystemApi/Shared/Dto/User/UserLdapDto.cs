using TrackingSystem.Api.Shared.Enums;

namespace TrackingSystem.Api.Shared.Dto.User
{
    public class UserLdapDto
    {
        public string? UserName { get; set; }

        public string? UserLogin { get; set; }

        public string? Password { get; set; }

        public string? Group { get; set; }

        public EStatus Status { get; set; }

        public ICollection<ERoles> Roles { get; set; }
    }
}
