using TrackingSystem.Api.Shared.Dto.User;

namespace TrackingSystem.Api.Shared.IManagers
{
    public interface IUserDbManager
    {
        Task<UserResponseData> FindUser(UserDataQuery query, CancellationToken cancellationToken);

        Task<UserByIdResponse?> FindUserById(UserByIdQuery request, CancellationToken cancellationToken);
    }
}
