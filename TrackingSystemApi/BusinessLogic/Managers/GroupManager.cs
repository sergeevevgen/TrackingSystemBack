using TrackingSystem.Api.Shared.Dto.Group;
using TrackingSystem.Api.Shared.Dto.Lesson;
using TrackingSystem.Api.Shared.IManagers.DbManagers;
using TrackingSystem.Api.Shared.IManagers.LogicManagers;
using TrackingSystem.Api.Shared.SharedModels;

namespace TrackingSystem.Api.BusinessLogic.Managers
{
    public class GroupManager : IGroupManager
    {
        private readonly ILogger _logger;
        private readonly IGroupDbManager _storage;

        public GroupManager(
            ILogger logger,
            IGroupDbManager manager)
        {
            _logger = logger;
            _storage = manager;
        }

        public async Task<GroupResponseDto> CreateOrUpdate(GroupDto model, CancellationToken cancellationToken)
        {
            var element = await _storage.GetElement(new GroupDto
            {
                Name = model.Name,
            }, cancellationToken);

            if (element != null && element.Id != model.Id)
            {
                _logger.Error("Уже есть группа с такими названием");
                throw new Exception("Уже есть группа с таким названием");
            }

            if (model.Id.HasValue)
            {
                element = await _storage.Update(model, cancellationToken);
                _logger.Info($"Группа с идентификатором {model.Id} обновлена");
            }
            else
            {
                element = await _storage.Insert(model, cancellationToken);
                _logger.Info($"Создана новая группа");
            }

            return element;
        }

        public async Task<bool> Delete(GroupDto model, CancellationToken cancellationToken)
        {
            _ = await _storage.GetElement(new GroupDto
            {
                Id = model.Id,
            }, cancellationToken) ?? throw new Exception($"Элемент с идентификатором {model.Id} не найден");

            await _storage.Delete(model, cancellationToken);

            return true;
        }

        public async Task<ResponseModel<GroupResponseDto>> Read(GroupDto model, CancellationToken cancellationToken)
        {
            if (model.Id.HasValue)
            {
                var data = await _storage.GetElement(model, cancellationToken);
                return new ResponseModel<GroupResponseDto> { Data = data };
            }

            return new ResponseModel<GroupResponseDto> { ErrorMessage = $"Такая группа не найдена {model.Name}" };
        }

        public Task<ResponseModel<List<GroupResponseDto>>> ReadAll(List<GroupDto> model, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
