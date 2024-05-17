using TrackingSystem.Api.Shared.Dto.Group;

namespace TrackingSystem.Api.Shared.IManagers.DbManagers
{
    public interface IGroupDbManager
    {
        Task<GroupResponseDto> Insert(GroupDto model, CancellationToken cancellationToken = default);

        Task<GroupResponseDto> Update(GroupDto model, CancellationToken cancellationToken = default);

        Task<bool> Delete(GroupDto model, CancellationToken cancellationToken = default);

        Task<GroupResponseDto?> GetElement(GroupDto model, CancellationToken cancellationToken = default);
    }
}