using TrackingSystem.Api.Shared.Dto.Place;

namespace TrackingSystem.Api.Shared.IManagers.DbManagers
{
    public interface IPlaceDbManager
    {
        Task<PlaceResponseDto> Insert(PlaceDto model, CancellationToken cancellationToken = default);

        Task<PlaceResponseDto> Update(PlaceDto model, CancellationToken cancellationToken = default);

        Task<bool> Delete(PlaceDto model, CancellationToken cancellationToken = default);

        Task<PlaceResponseDto?> GetElement(PlaceDto model, CancellationToken cancellationToken = default);
    }
}
