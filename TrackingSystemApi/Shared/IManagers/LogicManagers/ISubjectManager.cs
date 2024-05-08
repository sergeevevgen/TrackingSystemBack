using TrackingSystem.Api.Shared.Dto.Subject;
using TrackingSystem.Api.Shared.Dto.User;
using TrackingSystem.Api.Shared.SharedModels;

namespace TrackingSystem.Api.Shared.IManagers.LogicManagers
{
    public interface ISubjectManager
    {
        /// <summary>
        /// Метод для создания или обновления занятия
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<SubjectResponseDto> CreateOrUpdate(SubjectDto model, CancellationToken cancellationToken);

        /// <summary>
        /// Метод для удаления занятия
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> Delete(SubjectDto model, CancellationToken cancellationToken);

        /// <summary>
        /// Метод для получения одного занятия
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ResponseModel<SubjectResponseDto>> Read(SubjectDto model, CancellationToken cancellationToken);

        /// <summary>
        /// Метод для получения занятия по идентификаторам
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ResponseModel<List<SubjectResponseDto>>> ReadAll(List<SubjectDto> model, CancellationToken cancellationToken);

        /// <summary>
        /// Метод для получения отметки присутствия на занятии
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ResponseModel<SubjectResponseDto>> MarkSubject(UserMarkDto model, CancellationToken cancellationToken);
    }
}
