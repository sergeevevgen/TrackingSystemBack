using TrackingSystem.Api.Shared.Dto.Place;

namespace TrackingSystem.Api.Shared.IManagers.DbManagers
{
    public interface IPlaceDbManager
    {
        Task<PlaceResponseDto> Insert(PlaceDto model, CancellationToken cancellationToken);

        Task<PlaceResponseDto> Update(PlaceDto model, CancellationToken cancellationToken);

        Task<bool> Delete(PlaceDto model, CancellationToken cancellationToken);

        Task<PlaceResponseDto?> GetElement(PlaceDto model, CancellationToken cancellationToken);
    }
}
