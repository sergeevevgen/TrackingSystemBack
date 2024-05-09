using TrackingSystem.Api.Shared.Dto.Subject;

namespace TrackingSystem.Api.Shared.IManagers.DbManagers
{
    public interface ISubjectDbManager
    {
        Task<SubjectResponseDto> Insert(SubjectDto model, CancellationToken cancellationToken);

        Task<SubjectResponseDto> Update(SubjectDto model, CancellationToken cancellationToken);

        Task<bool> Delete(SubjectDto model, CancellationToken cancellationToken);

        Task<SubjectResponseDto?> GetElement(SubjectDto model, CancellationToken cancellationToken);

        Task<bool> MarkUserSubject(SubjectUserMarkDto model, CancellationToken cancellationToken);

        Task ChangeIsDifference(SubjectChangeIsDifferenceByWeekDto model, CancellationToken cancellationToken);

        Task DeleteExpired(SubjectChangeIsDifferenceByWeekDto model, CancellationToken cancellationToken);
    }
}
