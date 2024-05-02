using TrackingSystem.Api.Shared.Dto.Group;

namespace TrackingSystem.Api.Shared.IManagers
{
    public interface IGroupDbManager
    {
        Task Insert(GroupDto model, CancellationToken cancellationToken);

        Task Update(GroupDto model, CancellationToken cancellationToken);

        Task Delete(GroupDto model, CancellationToken cancellationToken);

        Task<GroupResponseDto?> GetElement(GroupDto model, CancellationToken cancellationToken);
    }
}