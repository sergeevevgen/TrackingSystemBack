using TrackingSystem.Api.Shared.Dto.Lesson;
using TrackingSystem.Api.Shared.Dto.User;
using TrackingSystem.Api.Shared.IManagers.DbManagers;
using TrackingSystem.Api.Shared.IManagers.LogicManagers;
using TrackingSystem.Api.Shared.SharedModels;

namespace TrackingSystem.Api.BusinessLogic.Managers
{
    public class LessonManager : ILessonManager
    {
        private readonly ILogger _logger;
        private readonly ILessonDbManager _storage;

        public LessonManager(
            ILogger logger,
            ILessonDbManager manager)
        {
            _logger = logger;
            _storage = manager;
        }

        public async Task<LessonResponseDto> CreateOrUpdate(LessonDto model, CancellationToken cancellationToken = default)
        {
            var element = await _storage.GetElement(new LessonDto
            {
                Name = model.Name,
            }, cancellationToken);

            if (element != null && element.Id != model.Id)
            {
                _logger.Error("Уже есть тип занятия с такими названием");
                throw new Exception("Уже есть тип занятия с таким названием");
            }

            if (model.Id.HasValue)
            {
                element = await _storage.Update(model, cancellationToken);
                _logger.Info($"Тип занятия с идентификатором {model.Id} обновлен");
            }
            else
            {
                element = await _storage.Insert(model, cancellationToken);
                _logger.Info($"Создан новый тип занятия");
            }

            return element;
        }

        public async Task<bool> Delete(LessonDto model, CancellationToken cancellationToken = default)
        {
            _ = await _storage.GetElement(new LessonDto
            {
                Id = model.Id,
            }, cancellationToken) ?? throw new Exception($"Элемент с идентификатором {model.Id} не найден");

            await _storage.Delete(model, cancellationToken);
            
            return true;
        }

        public async Task<ResponseModel<LessonResponseDto>> Read(LessonDto model, CancellationToken cancellationToken = default)
        {
            if (model.Id.HasValue)
            {
                var data = await _storage.GetElement(model, cancellationToken);
                return new ResponseModel<LessonResponseDto> { Data = data };
            }

            return new ResponseModel<LessonResponseDto> { ErrorMessage = $"Такой тип занятия не найден {model.Name}" };
        }

        public Task<ResponseModel<List<LessonResponseDto>>> ReadAll(List<LessonDto> model, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
