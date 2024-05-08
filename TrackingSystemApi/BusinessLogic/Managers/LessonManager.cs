using TrackingSystem.Api.Shared.Dto.Lesson;
using TrackingSystem.Api.Shared.IManagers.DbManagers;
using TrackingSystem.Api.Shared.IManagers.LogicManagers;
using TrackingSystem.Api.Shared.SharedModels;

namespace TrackingSystem.Api.BusinessLogic.Managers
{
    public class LessonManager : ILessonManager
    {
        private readonly ILogger _logger;
        private readonly ILessonDbManager _manager;

        public LessonManager(
            ILogger logger,
            ILessonDbManager manager)
        {
            _logger = logger;
            _manager = manager;
        }

        public Task<LessonResponseDto> CreateOrUpdate(LessonDto model, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(LessonDto model, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel<LessonResponseDto>> Read(LessonDto model, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel<List<LessonResponseDto>>> ReadAll(List<LessonDto> model, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
