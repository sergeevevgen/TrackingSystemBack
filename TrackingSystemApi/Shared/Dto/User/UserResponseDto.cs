using TrackingSystem.Api.Shared.Enums;

namespace TrackingSystem.Api.Shared.Dto.User
{
    /// <summary>
    /// Dto для ответа на запрос о получении данных пользователя
    /// </summary>
    public class UserResponseDto
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Логин
        /// </summary>
        public string Login { get; set; } = null!;

        /// <summary>
        /// Роли
        /// </summary>
        public List<ERoles>? Roles { get; set; }

        /// <summary>
        /// ФИО пользователя
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Учебная группа
        /// </summary>
        public string? Group { get; set; }

        /// <summary>
        /// Идентификатор учебной группы (если его нет, то этот пользователь - учитель)
        /// </summary>
        public Guid? GroupId { get; set; }

        /// <summary>
        /// Статус (отчислен, вышел из академа, учится)
        /// </summary>
        public EStatus? Status { get; set; }
    }
}
