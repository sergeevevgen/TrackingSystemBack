using TrackingSystem.Api.Shared.Dto.Lesson;
using TrackingSystem.Api.Shared.Dto.Place;
using TrackingSystem.Api.Shared.IManagers.DbManagers;
using TrackingSystem.Api.Shared.IManagers.LogicManagers;
using TrackingSystem.Api.Shared.SharedModels;

namespace TrackingSystem.Api.BusinessLogic.Managers
{
    public class PlaceManager : IPlaceManager
    {
        private readonly ILogger _logger;
        private readonly IPlaceDbManager _storage;

        public PlaceManager(
            ILogger logger,
            IPlaceDbManager manager)
        {
            _logger = logger;
            _storage = manager;
        }

        public async Task<PlaceResponseDto> CreateOrUpdate(PlaceDto model, CancellationToken cancellationToken)
        {
            var element = await _storage.GetElement(new PlaceDto
            {
                Name = model.Name,
            }, cancellationToken);

            if (element != null && element.Id != model.Id)
            {
                _logger.Error("Уже есть помещение с таким названием");
                throw new Exception("Уже есть помещение с таким названием");
            }

            if (model.Id.HasValue)
            {
                element = await _storage.Update(model, cancellationToken);
                _logger.Info($"Помещение с идентификатором {model.Id} обновлен");
            }
            else
            {
                element = await _storage.Insert(model, cancellationToken);
                _logger.Info($"Создано новое помещение");
            }

            return element;
        }

        public async Task<bool> Delete(PlaceDto model, CancellationToken cancellationToken)
        {
            _ = await _storage.GetElement(new PlaceDto
            {
                Id = model.Id,
            }, cancellationToken) ?? throw new Exception($"Элемент с идентификатором {model.Id} не найден");

            await _storage.Delete(model, cancellationToken);

            return true;
        }

        public async Task<ResponseModel<PlaceResponseDto>> Read(PlaceDto model, CancellationToken cancellationToken)
        {
            if (model.Id.HasValue)
            {
                var data = await _storage.GetElement(model, cancellationToken);
                return new ResponseModel<PlaceResponseDto> { Data = data };
            }

            return new ResponseModel<PlaceResponseDto> { ErrorMessage = $"Такое помещение не найдено {model.Name}" };
        }

        public Task<ResponseModel<PlaceResponseDto>> ReadAll(List<PlaceDto> model, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
