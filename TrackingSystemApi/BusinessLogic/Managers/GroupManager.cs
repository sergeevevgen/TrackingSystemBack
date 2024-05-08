using TrackingSystem.Api.Shared.Dto.Group;
using TrackingSystem.Api.Shared.IManagers.DbManagers;
using TrackingSystem.Api.Shared.IManagers.LogicManagers;
using TrackingSystem.Api.Shared.SharedModels;

namespace TrackingSystem.Api.BusinessLogic.Managers
{
    public class GroupManager : IGroupManager
    {
        private readonly ILogger _logger;
        private readonly IGroupDbManager _manager;

        public GroupManager(
            ILogger logger,
            IGroupDbManager manager)
        {
            _logger = logger;
            _manager = manager;
        }

        public Task<GroupResponseDto> CreateOrUpdate(GroupDto model, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(GroupDto model, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel<GroupResponseDto>> Read(GroupDto model, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel<List<GroupResponseDto>>> ReadAll(List<GroupDto> model, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
