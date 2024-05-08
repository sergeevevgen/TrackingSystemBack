using TrackingSystem.Api.Shared.Dto.Lesson;
using TrackingSystem.Api.Shared.SharedModels;

namespace TrackingSystem.Api.Shared.IManagers.LogicManagers
{
    public interface ILessonManager
    {
        /// <summary>
        /// Метод для создания или обновления типа помещения
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<LessonResponseDto> CreateOrUpdate(LessonDto model, CancellationToken cancellationToken);

        /// <summary>
        /// Метод для удаления типа помещения
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> Delete(LessonDto model, CancellationToken cancellationToken);

        /// <summary>
        /// Метод для получения одного типа помещения
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ResponseModel<LessonResponseDto>> Read(LessonDto model, CancellationToken cancellationToken);

        /// <summary>
        /// Метод для получения типов помещений по идентификаторам
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ResponseModel<List<LessonResponseDto>>> ReadAll(List<LessonDto> model, CancellationToken cancellationToken);
    }
}
