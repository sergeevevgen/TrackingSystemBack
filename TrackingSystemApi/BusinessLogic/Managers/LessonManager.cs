﻿    using TrackingSystem.Api.Shared.Dto.Lesson;
using TrackingSystem.Api.Shared.Dto.Subject;
using TrackingSystem.Api.Shared.Dto.User;
using TrackingSystem.Api.Shared.IManagers.DbManagers;
using TrackingSystem.Api.Shared.IManagers.LogicManagers;
using TrackingSystem.Api.Shared.SharedModels;

namespace TrackingSystem.Api.BusinessLogic.Managers
{
    public class LessonManager : ILessonManager
    {
        private readonly ILogger _logger;
        private readonly ILessonDbManager _storage;

        public LessonManager(
            ILogger logger,
            ILessonDbManager manager)
        {
            _logger = logger;
            _storage = manager;
        }

        public async Task<LessonResponseDto> CreateOrUpdate(LessonDto model, CancellationToken cancellationToken = default)
        {
            var element = await _storage.GetElement(new LessonDto
            {
                Name = model.Name,
            }, cancellationToken);

            if (element != null && element.Id != model.Id)
            {
                _logger.Error("Уже есть тип занятия с такими названием");
                throw new Exception("Уже есть тип занятия с таким названием");
            }

            if (model.Id.HasValue)
            {
                element = await _storage.Update(model, cancellationToken);
                _logger.Info($"Тип занятия с идентификатором {model.Id} обновлен");
            }
            else
            {
                element = await _storage.Insert(model, cancellationToken);
                _logger.Info($"Создан новый тип занятия");
            }

            return element;
        }

        public async Task<bool> Delete(LessonDto model, CancellationToken cancellationToken = default)
        {
            _ = await _storage.GetElement(new LessonDto
            {
                Id = model.Id,
            }, cancellationToken) ?? throw new Exception($"Элемент с идентификатором {model.Id} не найден");

            await _storage.Delete(model, cancellationToken);
            
            return true;
        }

        public async Task<ResponseModel<List<LessonsByTeacherResponseDto>>> GetTeacherLessons(Guid teacherId, CancellationToken cancellationToken = default)
        {
            try
            {
                var lessons = await _storage.GetTeacherLessons(teacherId);

                if (lessons is null)
                {
                    var errorMessage = "Не удалось получить занятия преподавателя";
                    _logger.Error(errorMessage);

                    return new ResponseModel<List<LessonsByTeacherResponseDto>> { ErrorMessage = errorMessage };
                }

                return new ResponseModel<List<LessonsByTeacherResponseDto>> { Data = lessons };
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                return new ResponseModel<List<LessonsByTeacherResponseDto>> { ErrorMessage = ex.Message };
            }
        }

        public async Task<ResponseModel<LessonsStatisticDto>> GetTeacherLessonStatistic(TeacherLessonStatisticDto model, CancellationToken cancellationToken = default)
        {
            try
            {
                var lessons = await _storage.GetTeacherLessonStatistic(model);

                if (lessons is null || lessons.Students.Count <= 0)
                {
                    var errorMessage = "Никто еще не отметился на этом занятии";
                    _logger.Error(errorMessage);

                    return new ResponseModel<LessonsStatisticDto> { ErrorMessage = errorMessage };
                }

                return new ResponseModel<LessonsStatisticDto> { Data = lessons };
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                return new ResponseModel<LessonsStatisticDto> { ErrorMessage = ex.Message };
            }
        }

        public async Task<ResponseModel<LessonResponseDto>> Read(LessonDto model, CancellationToken cancellationToken = default)
        {
            try
            {
                var data = await _storage.GetElement(model, cancellationToken);

                return new ResponseModel<LessonResponseDto> { Data = data };
            }
            catch (Exception ex)
            {
                return new ResponseModel<LessonResponseDto> { ErrorMessage = ex.Message};
            }
        }

        public Task<ResponseModel<List<LessonResponseDto>>> ReadAll(List<LessonDto> model, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
