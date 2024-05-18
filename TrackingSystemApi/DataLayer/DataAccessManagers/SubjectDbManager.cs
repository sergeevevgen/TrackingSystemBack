using TrackingSystem.Api.DataLayer.Data;
using TrackingSystem.Api.Shared.Dto.Subject;
using TrackingSystem.Api.Shared.IManagers.DbManagers;
using Microsoft.EntityFrameworkCore;
using TrackingSystem.Api.DataLayer.Models;
using TrackingSystem.Api.Shared.Enums;
using TrackingSystem.Api.Shared.Dto.User;
using Microsoft.IdentityModel.Tokens;

namespace TrackingSystem.Api.DataLayer.DataAccessManagers
{
    public class SubjectDbManager : ISubjectDbManager
    {
        private readonly ILogger _logger;
        private readonly TrackingSystemContext _context;

        public SubjectDbManager(
            ILogger logger,
            TrackingSystemContext context)
        {
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// Метод для удаления занятия
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> Delete(SubjectDto model, CancellationToken cancellationToken = default)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                Subject element = await _context.Subjects
                    .FirstOrDefaultAsync(s => s.Id.Equals(model.Id.Value), cancellationToken);
                
                if (element != null)
                {
                    _context.Subjects.Remove(element);
                    await _context.SaveChangesAsync(cancellationToken);
                    await transaction.CommitAsync(cancellationToken);
                    return true;
                }
                else
                {
                    throw new Exception("Элемент не найден");
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.Error(ex, $"Ошибка удаления занятия c Id {model.Id}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Метод для получения занятия
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<SubjectResponseDto?> GetElement(SubjectDto model, CancellationToken cancellationToken = default)
        {
            try
            {
                // Если не по идентификатору, то по всем остальным полям
                var query = _context.Subjects
                    .AsNoTracking()
                    .AsQueryable();

                if (model.Id.HasValue)
                {
                    query = query.Where(s => s.Id.Equals(model.Id.Value))
                        .Include(s => s.Group)
                        .Include(s => s.Lesson)
                        .Include(s => s.Place)
                        .Include(s => s.Users);
                }
                else if (!string.IsNullOrEmpty(model.Type))
                {
                    query = query
                        .Where(s =>
                            s.Week.Equals(model.Week) &&
                            s.Day.Equals(model.Day) &&
                            s.Type.Equals(model.Type) &&
                            s.Pair.Equals(model.Pair) &&
                            s.GroupId.Equals(model.GroupId) &&
                            s.PlaceId.Equals(model.PlaceId) &&
                            s.LessonId.Equals(model.LessonId) &&
                            s.TeacherId.Equals(model.TeacherId)
                        )
                        .Include(s => s.Group)
                        .Include(s => s.Lesson)
                        .Include(s => s.Place)
                        .Include(s => s.Users);
                }
                else
                {
                    return null;
                }
                    
                var element = await query.FirstOrDefaultAsync(cancellationToken);

                return element == null ? null : CreateModel(element);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Ошибка получения занятия c Id {model.Id}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Метод для создания занятия
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<SubjectResponseDto> Insert(SubjectDto model, CancellationToken cancellationToken = default)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                Subject subject = new()
                {
                    Week = model.Week,
                    Day = model.Day,
                    Type = model.Type,
                    Pair = model.Pair,
                    IsDifference = model.IsDifference,
                    GroupId = model.GroupId,
                    LessonId = model.LessonId,
                    PlaceId = model.PlaceId,
                    TeacherId = model.TeacherId,
                };

                await _context.Subjects.AddAsync(subject);
                await _context.SaveChangesAsync(cancellationToken);

                await CreateModel(model, subject, _context);
                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return CreateModel(subject);
            }
            catch(Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.Error(ex, $"Ошибка создания занятия: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Метод для обновления занятия
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<SubjectResponseDto> Update(SubjectDto model, CancellationToken cancellationToken = default)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var element = await _context.Subjects
                    .FirstOrDefaultAsync(u => u.Id.Equals(model.Id), cancellationToken) ?? throw new Exception($"Занятие с Id {model.Id} не найдено");

                await CreateModel(model, element, _context);
                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return CreateModel(element);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.Error(ex, $"Ошибка обновления занятия c Id {model.Id}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Метод для отметки посещения пользователя
        /// </summary> Продумать логику!!!!!
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> MarkUserSubject(SubjectUserMarkDto model, CancellationToken cancellationToken = default)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                // Получил элемент, который необходимо обновить
                var element = await _context.UserSubjects
                    .FirstOrDefaultAsync(u => u.SubjectId.Equals(model.SubjectId), cancellationToken) ?? throw new Exception($"Занятие с Id {model.SubjectId} не найдено");

                // Если нет такого, то создаем новую запись
                if (element == null)
                {
                    element = new()
                    {
                        SubjectId = model.SubjectId,
                        IsMarked = model.Mark,
                        MarkTime = model.MarkTime,
                        UserId = model.PupilId,
                    };
                    await _context.UserSubjects
                        .AddAsync(element, cancellationToken);
                }
                // Иначе обновляем поля
                else
                {
                    element.MarkTime = model.MarkTime;
                    element.IsMarked = model.Mark;
                }
                
                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.Error(ex, $"Ошибка обновления занятия c Id {model.SubjectId}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Метод для биндинга полей dto и модели db
        /// </summary>
        /// <param name="model"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        private static async Task<Subject> CreateModel(SubjectDto model, Subject subject, TrackingSystemContext context)
        {
            if (model.Id.HasValue)
            {
                subject.Week = model.Week;
                subject.Day = model.Day;
                subject.Type = model.Type;
                subject.Pair = model.Pair;
                subject.IsDifference = model.IsDifference;
                subject.GroupId = model.GroupId;
                subject.LessonId = model.LessonId;
                subject.PlaceId = model.PlaceId;
                subject.TeacherId = model.TeacherId;

                // Работа со связанными сущностями. Надо удалить те, которых нет и добавить новые
                ICollection<UserSubject> userSubjects = context.UserSubjects
                    .Where(r => r.SubjectId.Equals(model.Id.Value))
                    .ToList();

                context.UserSubjects
                    .RemoveRange(userSubjects
                        .Where(us => !model.Users.ContainsKey(us.UserId))
                        .ToList());

                await context.SaveChangesAsync();

                foreach (UserSubject us in userSubjects)
                {
                    us.IsMarked = model.Users[us.UserId];
                    us.MarkTime = DateTime.Now;
                    model.Users.Remove(us.UserId);
                }
                await context.SaveChangesAsync();
            }

            foreach (var u in model.Users)
            {
                await context.UserSubjects.AddAsync(new UserSubject
                {
                    SubjectId = subject.Id,
                    UserId = u.Key,
                    IsMarked = u.Value,
                    MarkTime = DateTime.Now
                });
                await context.SaveChangesAsync();
            }

            return subject;
        }

        /// <summary>
        /// Метод для биндинга полей модели db и полей dto
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private static SubjectResponseDto CreateModel(Subject subject)
        {
            return new SubjectResponseDto
            {
                Id = subject.Id,
                GroupId = subject.GroupId,
                PlaceId = subject.PlaceId,
                LessonId = subject.LessonId,
                TeacherId = subject.TeacherId,
                Day = subject.Day,
                Week = subject.Week,
                IsDifference = subject.IsDifference,
                Pair = subject.Pair,
                Type = subject.Type,
                Users = subject.Users
                    .ToDictionary(k => k.UserId, v => v.IsMarked)
            };
        }

        /// <summary>
        /// Метод для изменения IsDifference = 0 при начале нового парсинга для выбранной недели
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task ChangeIsDifference(SubjectChangeIsDifferenceByWeekDto model, CancellationToken cancellationToken = default)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                // Меняем isDifference = 0
                var query = await _context.Subjects
                    .Where(s =>
                    s.Week.Equals(model.Week))
                    .ToListAsync(cancellationToken);

                query.ForEach(s => s.IsDifference = EIsDifference.Expired);

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch(Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.Error(ex, $"Ошибка обновления ");
                throw;
            }
        }

        /// <summary>
        /// Метод для удаления занятий, которые не были тронуты при повторном парсинге
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task DeleteExpired(SubjectChangeIsDifferenceByWeekDto model, CancellationToken cancellationToken = default)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                // Удаляем те, у которых IsDifference == 0, совпадает неделя и день недели больше, чем тот, который на данный момент
                var query = _context.Subjects
                    .Where(s =>
                    s.Week.Equals(model.Week)
                    && s.IsDifference.Equals(EIsDifference.Expired)
                    && s.Day > ((int)DateTime.Today.DayOfWeek - 1 + 7) % 7);

                _context.Subjects.RemoveRange(query);

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.Error(ex, $"Ошибка удаления занятий недели #{model.Week} с {EIsDifference.Expired}");
                throw;
            }
        }

        public async Task<UserGetTimetableResponseDto> GetGroupTimetable(GroupGetTimetableDto model, CancellationToken cancellationToken = default)
        {
            try
            {
                // Получаем текущую неделю
                var week = await _context.Infos.FirstOrDefaultAsync(cancellationToken);

                var query = await (from subjects in _context.Subjects
                                   join lessons in _context.Lessons on subjects.LessonId equals lessons.Id
                                   join places in _context.Places on subjects.PlaceId equals places.Id
                                   join groups in _context.Groups on subjects.GroupId equals groups.Id
                                   where groups.Id.Equals(model.GroupId) && subjects.Week.Equals(week.Week)
                                   select new TimetableResponseDto
                                   {
                                       Day = subjects.Day,
                                       GroupName = groups.Name,
                                       LessonName = lessons.Name,
                                       Number = subjects.Pair,
                                       PlaceName = places.Name,
                                       TeacherName = subjects.Teacher.LastName + " " + subjects.Teacher.FirstName[0] + ". " + subjects.Teacher.MiddleName[0] + ".",
                                       Type = subjects.Type,
                                   }).ToListAsync(cancellationToken);

                return new UserGetTimetableResponseDto
                {
                    Week = week.Week,
                    Timetable = query,
                };
            }
            catch(Exception ex)
            {
                _logger.Error(ex, $"Ошибка получения занятий для группы с идентификатором {model.GroupId}");
                throw;
            }
        }
    }
}
