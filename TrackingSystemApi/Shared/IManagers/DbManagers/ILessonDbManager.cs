using TrackingSystem.Api.Shared.Dto.Lesson;

namespace TrackingSystem.Api.Shared.IManagers.DbManagers
{
    public interface ILessonDbManager
    {
        Task Insert(LessonDto model, CancellationToken cancellationToken);

        Task Update(LessonDto model, CancellationToken cancellationToken);

        Task Delete(LessonDto model, CancellationToken cancellationToken);

        Task<LessonResponseDto?> GetElement(LessonDto model, CancellationToken cancellationToken);
    }
}
