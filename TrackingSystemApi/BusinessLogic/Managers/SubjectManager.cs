using System.Web.Http.Metadata;
using TrackingSystem.Api.Shared.Dto.Subject;
using TrackingSystem.Api.Shared.Enums;
using TrackingSystem.Api.Shared.IManagers.DbManagers;
using TrackingSystem.Api.Shared.IManagers.LogicManagers;
using TrackingSystem.Api.Shared.SharedModels;

namespace TrackingSystem.Api.BusinessLogic.Managers
{
    public class SubjectManager : ISubjectManager
    {
        private readonly ILogger _logger;
        private readonly ISubjectDbManager _storage;

        public SubjectManager(
            ILogger logger,
            ISubjectDbManager storage)
        {
            _logger = logger;
            _storage = storage;
        }

        /// <summary>
        /// Метод создания или обновления занятия
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<SubjectResponseDto> CreateOrUpdate(SubjectDto model, CancellationToken cancellationToken = default)
        {
            var element = await _storage.GetElement(new SubjectDto
            {
                Day = model.Day,
                Week = model.Week,
                Pair = model.Pair,
                Type = model.Type,
                GroupId = model.GroupId,
                LessonId = model.LessonId,
                PlaceId = model.PlaceId,
                TeacherId = model.TeacherId
            }, cancellationToken);

            if (element != null && element.Id != model.Id)
            {
                _logger.Error("Уже есть занятие с такими параметрами");
                throw new Exception("Уже есть занятие с такими параметрами");
            }

            if (model.Id.HasValue)
            {
                model.IsDifference = EIsDifference.Actual;
                element = await _storage.Update(model, cancellationToken);
                _logger.Info($"Занятие с идентификатором {model.Id} обновлено");
            }
            else
            {
                element = await _storage.Insert(model, cancellationToken);
                _logger.Info($"Создано новое занятие");
            }

            return element;
        }

        /// <summary>
        /// Метод для изменения IsDifference = 2
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task ChangeIsDifferenceByWeek(SubjectChangeIsDifferenceByWeekDto model, CancellationToken cancellationToken = default)
        {
            try
            {
                await _storage.ChangeIsDifference(model, cancellationToken);
            }
            catch(Exception ex)
            {
                _logger.Error(ex, "Произошла ошибка при изменении isDifference");
                throw;
            }
            _logger.Info("Изменение isDifference прошло успешно");
        }

        /// <summary>
        /// Метод для изменения IsDifference = 2
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task DeleteExpired(SubjectChangeIsDifferenceByWeekDto model, CancellationToken cancellationToken = default)
        {
            try
            {
                await _storage.DeleteExpired(model, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Произошла ошибка при удалении исчезнувших занятий");
                throw;
            }
            _logger.Info("Удаление изчезнувших занятий прошло успешно");
        }

        /// <summary>
        /// Метод удаления занятия
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> Delete(SubjectDto model, CancellationToken cancellationToken = default)
        {
            _ = await _storage.GetElement(new SubjectDto
            {
                Id = model.Id
            }, cancellationToken) ?? throw new Exception($"Элемент с идентификатором {model.Id} не найден");

            await _storage.Delete(model, cancellationToken);
            return true;
        }

        /// <summary>
        /// Метод отметки присутствия на занятии
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ResponseModel<string>> MarkSubject(SubjectUserMarkDto model, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _storage.MarkUserSubject(model, cancellationToken);
                return new ResponseModel<string> { Data = $"Пользователь с идентификатором {model.PupilId} отметился на занятии с идентификатором {model.SubjectId}" };
            }
            catch (Exception ex)
            {
                return new ResponseModel<string> { ErrorMessage = ex.Message };
            } 
        }

        /// <summary>
        /// Метод получения занятия
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ResponseModel<SubjectResponseDto>> Read(SubjectDto model, CancellationToken cancellationToken = default)
        {
            if (model.Id.HasValue)
            {
                var data = await _storage.GetElement(model, cancellationToken);
                return new ResponseModel<SubjectResponseDto> { Data = data };
            }
            return new ResponseModel<SubjectResponseDto> { ErrorMessage = $"Такое занятие не найдено" };
        }

        /// <summary>
        /// Метод получения нескольких занятий
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<ResponseModel<List<SubjectResponseDto>>> ReadAll(List<SubjectDto> model, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
