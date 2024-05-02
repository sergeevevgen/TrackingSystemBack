using TrackingSystem.Api.Shared.Dto.Group;
using TrackingSystem.Api.Shared.IManagers;

namespace TrackingSystem.Api.DataLayer.DataAccessManagers
{
    /// <summary>
    /// Класс для взаимодействия с сущностью "Группа"
    /// </summary>
    public class GroupDbManager : IGroupDbManager
    {
        private readonly ILogger _logger;

        public GroupDbManager(
            ILogger logger)
        {
            _logger = logger;
        }

        public Task Delete(GroupDto model, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<GroupResponseDto?> GetElement(GroupDto model, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task Insert(GroupDto model, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task Update(GroupDto model, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
