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
        Task<SubjectResponseDto> CreateOrUpdate(SubjectDto model, CancellationToken cancellationToken = default);

        /// <summary>
        /// Метод для удаления занятия
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> Delete(SubjectDto model, CancellationToken cancellationToken = default);

        /// <summary>
        /// Метод для получения одного занятия
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ResponseModel<SubjectResponseDto>> Read(SubjectDto model, CancellationToken cancellationToken = default);

        /// <summary>
        /// Метод для получения занятия по идентификаторам
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ResponseModel<List<SubjectResponseDto>>> ReadAll(List<SubjectDto> model, CancellationToken cancellationToken = default);

        /// <summary>
        /// Метод для получения отметки присутствия на занятии
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<ResponseModel<string>> MarkSubject(SubjectUserMarkDto model, CancellationToken cancellationToken = default);

        /// <summary>
        /// Метод для подготовки занятий к новому парсингу. Ставим isDifference = 0
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task ChangeIsDifferenceByWeek(SubjectChangeIsDifferenceByWeekDto model, CancellationToken cancellationToken = default);

        /// <summary>
        /// Метод для удаления занятий, которые были изменены в расписании. У них флаг IsDifference = 0 после парсинга
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task DeleteExpired(SubjectChangeIsDifferenceByWeekDto model, CancellationToken cancellationToken = default);

        /// <summary>
        /// Метод для получения расписания на сегодня по пользователю
        /// </summary>
        /// <returns></returns>
        Task<ResponseModel<UserGetTimetableResponseDto>> GetTimetableToday(GroupGetTimetableDto model, CancellationToken cancellationToken = default);
    }
}
