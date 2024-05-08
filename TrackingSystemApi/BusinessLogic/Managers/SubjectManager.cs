using TrackingSystem.Api.Shared.Dto.Subject;
using TrackingSystem.Api.Shared.Dto.User;
using TrackingSystem.Api.Shared.IManagers.DbManagers;
using TrackingSystem.Api.Shared.IManagers.LogicManagers;
using TrackingSystem.Api.Shared.SharedModels;

namespace TrackingSystem.Api.BusinessLogic.Managers
{
    public class SubjectManager : ISubjectManager
    {
        private readonly ILogger _logger;
        private readonly ISubjectDbManager _manager;

        public SubjectManager(
            ILogger logger,
            ISubjectDbManager manager)
        {
            _logger = logger;
            _manager = manager;
        }

        public Task<SubjectResponseDto> CreateOrUpdate(SubjectDto model, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(SubjectDto model, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel<SubjectResponseDto>> MarkSubject(UserMarkDto model, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel<SubjectResponseDto>> Read(SubjectDto model, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel<List<SubjectResponseDto>>> ReadAll(List<SubjectDto> model, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
