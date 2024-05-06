using TrackingSystem.Api.Shared.Dto.Subject;
using TrackingSystem.Api.Shared.IManagers.DbManagers;

namespace TrackingSystem.Api.DataLayer.DataAccessManagers
{
    public class SubjectDbManager : ISubjectDbManager
    {
        public Task Delete(SubjectDto model, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<SubjectResponseDto?> GetElement(SubjectDto model, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task Insert(SubjectDto model, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task Update(SubjectDto model, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
