using TrackingSystem.Api.Shared.Dto.User;

namespace TrackingSystem.Api.Shared.IManagers.DbManagers
{
    /// <summary>
    /// Интерфейс для db-менеджера к пользователю
    /// </summary>
    public interface IUserDbManager
    {
        Task<UserResponseDto> FindUser(UserFindDto query, CancellationToken cancellationToken = default);

        Task<UserFindResponseDto?> FindUserById(UserFindDto request, CancellationToken cancellationToken = default);

        Task<UserResponseDto> Insert(UserDto model, CancellationToken cancellationToken = default);

        Task<UserResponseDto> Update(UserDto model, CancellationToken cancellationToken = default);

        Task<bool> Delete(UserDto model, CancellationToken cancellationToken = default);

        Task<UserResponseDto?> GetElement(UserDto model, CancellationToken cancellationToken = default);

        Task<UserResponseDto?> GetElementForLdap(UserDto model, CancellationToken cancellationToken = default);
    }
}
