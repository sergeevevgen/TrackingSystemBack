using TrackingSystem.Api.Shared.Dto.Place;

namespace TrackingSystem.Api.Shared.IManagers.DbManagers
{
    public interface IPlaceDbManager
    {
        Task Insert(PlaceDto model, CancellationToken cancellationToken);

        Task Update(PlaceDto model, CancellationToken cancellationToken);

        Task Delete(PlaceDto model, CancellationToken cancellationToken);

        Task<PlaceResponseDto?> GetElement(PlaceDto model, CancellationToken cancellationToken);
    }
}
