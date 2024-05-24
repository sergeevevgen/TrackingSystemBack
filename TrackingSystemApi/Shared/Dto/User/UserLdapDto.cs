using TrackingSystem.Api.Shared.Enums;

namespace TrackingSystem.Api.Shared.Dto.User
{
    public class UserLdapDto
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? MiddleName { get; set; }

        public string? UserLogin { get; set; }

        public string? Group { get; set; }

        public Status? Status { get; set; }

        public Role Role { get; set; }
    }
}
