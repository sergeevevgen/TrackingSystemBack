using TrackingSystem.Api.Shared.Dto.User;

namespace TrackingSystem.Api.Shared.IManagers.DbManagers
{
    /// <summary>
    /// Интерфейс для db-менеджера к пользователю
    /// </summary>
    public interface IUserDbManager
    {
        Task<UserResponseDto> FindUser(UserFindDto query, CancellationToken cancellationToken);

        Task<UserFindResponseDto?> FindUserById(UserFindDto request, CancellationToken cancellationToken);

        Task<UserResponseDto> Insert(UserDto model, CancellationToken cancellationToken);

        Task<UserResponseDto> Update(UserDto model, CancellationToken cancellationToken);

        Task<bool> Delete(UserDto model, CancellationToken cancellationToken);

        Task<UserResponseDto?> GetElement(UserDto model, CancellationToken cancellationToken);
    }
}
