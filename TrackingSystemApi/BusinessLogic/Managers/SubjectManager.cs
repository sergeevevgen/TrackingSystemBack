using System;
using TrackingSystem.Api.DataLayer.Models;
using TrackingSystem.Api.Shared.Dto.Group;
using TrackingSystem.Api.Shared.Dto.Subject;
using TrackingSystem.Api.Shared.Dto.User;
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
        private readonly IGroupDbManager _groupStorage;
        private readonly Dictionary<PairNumber, SubjectTimeSlotDto> SubjectsTime = new Dictionary<PairNumber, SubjectTimeSlotDto>
        {
            { PairNumber.First, new SubjectTimeSlotDto { StartTime = new TimeSpan(8, 30, 0), EndTime = new TimeSpan(9, 50, 0) } },
            { PairNumber.Second, new SubjectTimeSlotDto { StartTime = new TimeSpan(10, 0, 0), EndTime = new TimeSpan(11, 20, 0) } },
            { PairNumber.Third, new SubjectTimeSlotDto { StartTime = new TimeSpan(11, 30, 0), EndTime = new TimeSpan(12, 50, 0) } },
            { PairNumber.Fourth, new SubjectTimeSlotDto { StartTime = new TimeSpan(13, 30, 0), EndTime = new TimeSpan(14, 50, 0) } },
            { PairNumber.Fifth, new SubjectTimeSlotDto { StartTime = new TimeSpan(15, 0, 0), EndTime = new TimeSpan(16, 20, 0) } },
            { PairNumber.Sixth, new SubjectTimeSlotDto { StartTime = new TimeSpan(16, 30, 0), EndTime = new TimeSpan(17, 50, 0) } },
            { PairNumber.Seventh, new SubjectTimeSlotDto { StartTime = new TimeSpan(18, 0, 0), EndTime = new TimeSpan(19, 20, 0) } },
            { PairNumber.Eighth, new SubjectTimeSlotDto { StartTime = new TimeSpan(19, 30, 0), EndTime = new TimeSpan(20, 50, 0) } },
        };

        public SubjectManager(
            ILogger logger,
            ISubjectDbManager storage,
            IGroupDbManager dbStorage)
        {
            _logger = logger;
            _storage = storage;
            _groupStorage = dbStorage;
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
                model.IsDifference = Difference.Actual;
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
        public async Task<ResponseModel<SubjectUserMarkResponseDto>> MarkSubject(SubjectUserMarkDto model, CancellationToken cancellationToken = default)
        {
            try
            {
                var subject = await _storage.GetElement(new SubjectDto
                {
                    Id = model.SubjectId,
                });

                model.MarkTime = DateTime.Now;
                model.Mark = true;

                // Вытаскиваем инфу
                var info = await _storage.GetInfo();

                if (subject == null)
                {
                    return new ResponseModel<SubjectUserMarkResponseDto> { ErrorMessage = $"Занятие с {model.SubjectId} не найдено" };
                }

                // Пока отключим фильтры
                if (!subject.GroupId.Equals(model.GroupId))
                {
                    return new ResponseModel<SubjectUserMarkResponseDto> { ErrorMessage = $"У данного пользователя другая группа" };
                }

                //if (!subject.Week.Equals(info.Week))
                //{
                //    return new ResponseModel<SubjectUserMarkResponseDto> { ErrorMessage = $"Не та неделя" };
                //}

                //if (!IsMatchingDay(subject.Day, model.MarkTime.DayOfWeek))
                //{
                //    return new ResponseModel<SubjectUserMarkResponseDto> { ErrorMessage = $"Не тот день" };
                //}                

                //if (!CheckTimeMark(model.MarkTime, subject.Pair, info.AllowedDeviation))
                //{
                //    return new ResponseModel<SubjectUserMarkResponseDto> { ErrorMessage = $"Время на отметку на занятии прошло, вы опоздали" };
                //}

                var result = await _storage.MarkUserSubject(model, cancellationToken);

                // Если false, то это значит, что пользователь уже отметился
                if (!result)
                {
                    return new ResponseModel<SubjectUserMarkResponseDto> { ErrorMessage = "Вы уже отметились на данном занятии" };
                }

                var name = string.IsNullOrEmpty(subject.Type) ? null : subject.Type + ". ";
                return new ResponseModel<SubjectUserMarkResponseDto> 
                { 
                    Data = new SubjectUserMarkResponseDto 
                        {
                            SubjectName = name + subject.LessonName
                        }
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel<SubjectUserMarkResponseDto> { ErrorMessage = ex.Message };
            } 
        }

        private static bool IsMatchingDay(int customDay, DayOfWeek dayOfWeek)
        {
            // Преобразование DayOfWeek в ваш формат, где понедельник - это 0
            int dayOfWeekIndex = (int)dayOfWeek - 1;
            if (dayOfWeekIndex < 0)
            {
                dayOfWeekIndex = 6; // Если Sunday (6), корректируем на Saturday (6)
            }

            // Сравниваем преобразованный индекс с вашим значением
            return customDay == dayOfWeekIndex;
        }

        /// <summary>
        /// Метод для проверки времени отметки
        /// </summary>
        /// <param name="markTime"></param>
        /// <param name="number"></param>
        private bool CheckTimeMark(DateTime markTime, PairNumber number, TimeSpan allowedDeviation)
        {
            var startTime = SubjectsTime[number].StartTime;

            var markT = markTime.ToLocalTime().TimeOfDay;

            return markT >= startTime && markT <= startTime + allowedDeviation;
        }

        /// <summary>
        /// Метод получения занятия
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ResponseModel<SubjectResponseDto>> Read(SubjectDto model, CancellationToken cancellationToken = default)
        {
            try
            {
                var data = await _storage.GetElement(model, cancellationToken);

                return new ResponseModel<SubjectResponseDto> { Data = data };
            }
            catch (Exception ex)
            {
                return new ResponseModel<SubjectResponseDto> { ErrorMessage = ex.Message };
            }
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

        /// <summary>
        /// Метод получения расписания для текущей недели для ученика по его (GroupId)
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ResponseModel<UserGetTimetableResponseDto>> GetTimetableCurrentWeek(GroupGetTimetableDto model, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _storage.GetGroupTimetable(model, cancellationToken);

                if (result.Timetable.Count <= 0)
                {
                    return new ResponseModel<UserGetTimetableResponseDto> { Data = null };
                }

                return new ResponseModel<UserGetTimetableResponseDto> { Data = result };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return new ResponseModel<UserGetTimetableResponseDto> { ErrorMessage = $"Не удалось получить расписание для группы {model.GroupId}" + ex.Message };
            }
        }

        /// <summary>
        /// Метод получения расписания для учителя по его TeacherId
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ResponseModel<UserGetTimetableResponseDto>> GetTimetableCurrentWeekTeacher(TeacherGetTimetableDto model, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _storage.GetTeacherTimetable(model, cancellationToken);

                if (result.Timetable.Count <= 0)
                {
                    return new ResponseModel<UserGetTimetableResponseDto> { Data = null };
                }

                return new ResponseModel<UserGetTimetableResponseDto> { Data = result };
            }
            catch(Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return new ResponseModel<UserGetTimetableResponseDto> { ErrorMessage = $"Не удалось получить расписание для учителя {model.TeacherId}" + ex.Message };
            }
        }

        /// <summary>
        /// Метод для получения текущего занятия
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ResponseModel<SubjectTeacherResponseDto>> GetCurrentSubjectByTeacher(SubjectTeacherDto model, CancellationToken cancellationToken = default)
        {
            try
            {
                // Преобразование DayOfWeek в ваш формат, где понедельник - это 0
                int dayOfWeekIndex = (int)DateTime.Now.DayOfWeek;
                if (dayOfWeekIndex < 0)
                {
                    dayOfWeekIndex = 6; // Если Sunday (6), корректируем на Saturday (6)
                }

                model.Day = dayOfWeekIndex;

                var pair = GetCurrentOrNextPair();

                if (pair is null)
                {
                    throw new Exception("");
                }

                model.Pair = pair.Value + 1;
                var result = await _storage.GetCurrentSubjectByTeacher(model, cancellationToken);

                if (result is null)
                {
                    throw new Exception("");
                }

                var name = string.IsNullOrEmpty(result.Type) ? null : result.Type + ". ";

                return new ResponseModel<SubjectTeacherResponseDto>
                { 
                    Data = new SubjectTeacherResponseDto
                    {
                        SubjectId = result.Id,
                        SubjectName = name + result.LessonName,
                        GroupName = result.GroupName,
                        PlaceName = result.PlaceName,
                        From = SubjectsTime[result.Pair - 1].StartTime.ToString(),
                        To = SubjectsTime[result.Pair - 1].EndTime.ToString(),
                        Date = DateTime.Now.ToShortDateString(),
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return new ResponseModel<SubjectTeacherResponseDto> { ErrorMessage = $"Не удалось получить занятие для учителя {model.TeacherId}" + ex.Message };
            }
        }

        /// <summary>
        /// Метод для получения текущей пары
        /// </summary>
        /// <returns></returns>
        private PairNumber? GetCurrentOrNextPair()
        {
            var now = DateTime.Now.TimeOfDay;
            _logger.Info(now);
            foreach (var pair in SubjectsTime)
            {
                _logger.Info(pair);
                if (now >= pair.Value.StartTime && now <= pair.Value.EndTime)
                {
                    return pair.Key;
                }
                else if (now < pair.Value.StartTime)
                {
                    return pair.Key;
                }
            }

            return null;
        }
    }
}
