using TrackingSystem.Api.Shared.Dto.Lesson;

namespace TrackingSystem.Api.Shared.IManagers.DbManagers
{
    public interface ILessonDbManager
    {
        Task<LessonResponseDto> Insert(LessonDto model, CancellationToken cancellationToken);

        Task<LessonResponseDto> Update(LessonDto model, CancellationToken cancellationToken);

        Task<bool> Delete(LessonDto model, CancellationToken cancellationToken);

        Task<LessonResponseDto?> GetElement(LessonDto model, CancellationToken cancellationToken);
    }
}
