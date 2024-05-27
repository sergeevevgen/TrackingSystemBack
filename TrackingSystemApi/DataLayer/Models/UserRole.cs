namespace TrackingSystem.Api.DataLayer.Models
{
    /// <summary>
    /// Класс для связи Юзера и Роли
    /// </summary>
    public class UserRole
    {
        /// <summary>
        /// Внешний ключ на пользователя
        /// </summary>
        public Guid UserId { get; set; }
        public User User { get; set; }

        /// <summary>
        /// Внешний ключ на роль
        /// </summary>
        public Guid RoleId { get; set; }
        public Role Role { get; set; }
    }
}
