using TrackingSystem.Api.Shared.Dto.Subject;

namespace TrackingSystem.Api.Shared.IManagers.DbManagers
{
    public interface ISubjectDbManager
    {
        Task Insert(SubjectDto model, CancellationToken cancellationToken);

        Task Update(SubjectDto model, CancellationToken cancellationToken);

        Task Delete(SubjectDto model, CancellationToken cancellationToken);

        Task<SubjectResponseDto?> GetElement(SubjectDto model, CancellationToken cancellationToken);

        Task<bool> MarkUserSubject(SubjectUserMarkDto model, CancellationToken cancellationToken);

        Task ChangeIsDifference(SubjectChangeIsDifferenceByWeekDto model, CancellationToken cancellationToken);

        Task DeleteExpired(SubjectChangeIsDifferenceByWeekDto model, CancellationToken cancellationToken);
    }
}
