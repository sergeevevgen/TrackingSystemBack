using TrackingSystem.Api.Shared.Enums;

namespace TrackingSystem.Api.Shared.Dto.User
{
    public class UserLdapDto
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? MiddleName { get; set; }

        public string? UserLogin { get; set; }

        public string? Password { get; set; }

        public string? Group { get; set; }

        public EStatus? Status { get; set; }

        public ERoles Role { get; set; }
    }
}
