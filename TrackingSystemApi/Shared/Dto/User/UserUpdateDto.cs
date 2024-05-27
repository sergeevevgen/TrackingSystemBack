using TrackingSystem.Api.Shared.Enums;

namespace TrackingSystem.Api.Shared.Dto.User
{
    public class UserUpdateDto
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public string? Login { get; set; }

        public Guid? GroupId { get; set; }

        public Status Status { get; set; }

        public RoleEnum Role { get; set; }
    }
}
