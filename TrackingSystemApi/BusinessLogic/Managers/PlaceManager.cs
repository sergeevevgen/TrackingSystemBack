using TrackingSystem.Api.Shared.Dto.Place;
using TrackingSystem.Api.Shared.IManagers.DbManagers;
using TrackingSystem.Api.Shared.IManagers.LogicManagers;
using TrackingSystem.Api.Shared.SharedModels;

namespace TrackingSystem.Api.BusinessLogic.Managers
{
    public class PlaceManager : IPlaceManager
    {
        private readonly ILogger _logger;
        private readonly IPlaceDbManager _manager;

        public PlaceManager(
            ILogger logger,
            IPlaceDbManager manager)
        {
            _logger = logger;
            _manager = manager;
        }

        public Task<PlaceResponseDto> CreateOrUpdate(PlaceDto model, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(PlaceDto model, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel<PlaceResponseDto>> Read(PlaceDto model, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel<PlaceResponseDto>> ReadAll(List<PlaceDto> model, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
