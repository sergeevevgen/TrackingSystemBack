using TrackingSystem.Api.Shared.Dto.Group;
using TrackingSystem.Api.Shared.Dto.Subject;
using TrackingSystem.Api.Shared.Dto.User;

namespace TrackingSystem.Api.Shared.IManagers.DbManagers
{
    public interface ISubjectDbManager
    {
        Task<SubjectResponseDto> Insert(SubjectDto model, CancellationToken cancellationToken = default);

        Task<SubjectResponseDto> Update(SubjectDto model, CancellationToken cancellationToken = default);

        Task<bool> Delete(SubjectDto model, CancellationToken cancellationToken = default);

        Task<SubjectResponseDto?> GetElement(SubjectDto model, CancellationToken cancellationToken = default);

        Task<bool> MarkUserSubject(SubjectUserMarkDto model, CancellationToken cancellationToken = default);

        Task ChangeIsDifference(SubjectChangeIsDifferenceByWeekDto model, CancellationToken cancellationToken = default);

        Task DeleteExpired(SubjectChangeIsDifferenceByWeekDto model, CancellationToken cancellationToken = default);

        Task<UserGetTimetableResponseDto> GetGroupTimetable(GroupGetTimetableDto model, CancellationToken cancellationToken = default);

        Task<UserGetTimetableResponseDto> GetTeacherTimetable(TeacherGetTimetableDto model, CancellationToken cancellationToken = default);

        Task<InfoResponseDto> GetInfo(CancellationToken cancellationToken = default);

        Task<bool> ChangeInfo(InfoChangeDto model, CancellationToken cancellationToken = default);
    }
}
