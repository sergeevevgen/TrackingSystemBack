using TrackingSystem.Api.Shared.Enums;

namespace TrackingSystem.Api.Shared.Dto.User
{
    /// <summary>
    /// Dto для пользователя
    /// </summary>
    public class UserDto
    {
        public Guid? Id { get; set; }

        public string? Name { get; set; }

        public string? Login { get; set; }

        public string? Password { get; set; }

        public Guid? GroupId { get; set; }

        public EStatus Status { get; set; }

        public ICollection<Guid>? Roles { get; set; }
    }
}
