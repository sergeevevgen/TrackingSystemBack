using TrackingSystem.Api.Shared.Enums;

namespace TrackingSystem.Api.Shared.Dto.User
{
    /// <summary>
    /// Dto для пользователя
    /// </summary>
    public class UserDto
    {
        public Guid? Id { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? MiddleName { get; set; }

        public string? Login { get; set; }

        public Guid? GroupId { get; set; }

        public Status? Status { get; set; }

        public Role Role { get; set; }
    }
}
