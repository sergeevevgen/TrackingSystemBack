using TrackingSystem.Api.Shared.Dto.Lesson;

namespace TrackingSystem.Api.Shared.IManagers.DbManagers
{
    public interface ILessonDbManager
    {
        Task<LessonResponseDto> Insert(LessonDto model, CancellationToken cancellationToken = default);

        Task<LessonResponseDto> Update(LessonDto model, CancellationToken cancellationToken = default);

        Task<bool> Delete(LessonDto model, CancellationToken cancellationToken = default);

        Task<LessonResponseDto?> GetElement(LessonDto model, CancellationToken cancellationToken = default);

        Task<List<LessonsByTeacherResponseDto>> GetTeacherLessons(Guid teacherId, CancellationToken cancellationToken = default);
    }
}
