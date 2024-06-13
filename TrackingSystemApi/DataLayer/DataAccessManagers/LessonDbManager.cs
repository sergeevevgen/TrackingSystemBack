using TrackingSystem.Api.Shared.IManagers.DbManagers;
using Microsoft.EntityFrameworkCore;
using TrackingSystem.Api.DataLayer.Data;
using TrackingSystem.Api.DataLayer.Models;
using TrackingSystem.Api.Shared.Dto.Lesson;

namespace TrackingSystem.Api.DataLayer.DataAccessManagers
{
    public class LessonDbManager : ILessonDbManager
    {
        private readonly ILogger _logger;
        private readonly TrackingSystemContext _context;

        public LessonDbManager(
            ILogger logger,
            TrackingSystemContext context)
        {
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// Метод для удаления типа занятия
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<bool> Delete(LessonDto model, CancellationToken cancellationToken = default)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                Lesson element = await _context.Lessons
                    .FirstOrDefaultAsync(g => g.Id.Equals(model.Id.Value)
                    || g.Name.Equals(model.Name),
                    cancellationToken);

                if (element != null)
                {
                    _context.Lessons.Remove(element);
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
                _logger.Error(ex, $"Ошибка удаления типа задания c Id {model.Id}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Метод для получения типа занятия по идентификатору или названию
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<LessonResponseDto?> GetElement(LessonDto model, CancellationToken cancellationToken = default)
        {
            try
            {
                var query = _context.Lessons
                    .AsNoTracking()
                    .AsQueryable();

                if (model.Id.HasValue)
                {
                    query = query.Where(l => l.Id.Equals(model.Id.Value));
                }
                else if (!string.IsNullOrEmpty(model.Name))
                {
                    query = query.Where(l => l.Name.Contains(model.Name));
                }
                else
                {
                    return null;
                }

                var element = await query
                    .Include(g => g.Subjects)
                    .FirstOrDefaultAsync(cancellationToken);

                return element == null ? null : CreateModel(element);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Ошибка получения типа задания c Id {model.Id}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Метод для создания типа занятия
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<LessonResponseDto> Insert(LessonDto model, CancellationToken cancellationToken = default)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                Lesson lesson = new();

                await _context.Lessons.AddAsync(CreateModel(model, lesson));
                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return CreateModel(lesson);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.Error(ex, $"Ошибка создания типа задания: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Метод для обновления типа занятия
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<LessonResponseDto> Update(LessonDto model, CancellationToken cancellationToken = default)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var element = await _context.Lessons
                    .FirstOrDefaultAsync(g => g.Id.Equals(model.Id.Value), cancellationToken) ?? throw new Exception($"Тип задания с Id {model.Id} не найден");

                CreateModel(model, element);
                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return CreateModel(element);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.Error(ex, $"Ошибка обновления типа задания c Id {model.Id}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Метод для получения занятий, проводимых преподавателем
        /// </summary>
        /// <param name="teacherId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<List<LessonsByTeacherResponseDto>> GetTeacherLessons(Guid teacherId, CancellationToken cancellationToken = default)
        {
            try
            {
                var query = await (from subjects in _context.Subjects
                                   join lessons in _context.Lessons on subjects.LessonId equals lessons.Id
                                   where subjects.TeacherId.Equals(teacherId)
                                   orderby lessons.Id
                                   select new LessonsByTeacherResponseDto
                                   {
                                       LessonId = lessons.Id,
                                       LessonName = lessons.Name,
                                   })
                                   .GroupBy(x => x.LessonId)
                                   .Select(g => g.First())
                                   .ToListAsync(cancellationToken);

                return query;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Ошибка получения занятий для учителя с идентификатором {teacherId}");
                throw;
            }
        }

        public async Task<LessonsStatisticDto> GetTeacherLessonStatistic(TeacherLessonStatisticDto model, CancellationToken cancellationToken = default)
        {
            try
            {
                var info = await _context.Infos.OrderBy(i => i.Id).FirstOrDefaultAsync(cancellationToken);
                if (info == null)
                {
                    return null;
                }

                var query = await (from subjects in _context.Subjects
                                   join groups in _context.Groups on subjects.GroupId equals groups.Id
                                   join users in _context.Users on groups.Id equals users.GroupId
                                   where subjects.LessonId.Equals(model.LessonId) && subjects.TeacherId.Equals(model.TeacherId) 
                                   && subjects.Week.Equals(info.Week)
                                   select new 
                                   {
                                       subjects.GroupId,
                                       users.Id,
                                   })
                                   .ToListAsync(cancellationToken);
                
                // Получили количество учеников, у которых данное занятие на текущей неделе
                var count = query.Count;

                // Теперь вытащим количество отметок из UserSubjects
                var query2 = await (from subjects in _context.Subjects
                                    join us in _context.UserSubjects on subjects.Id equals us.SubjectId
                                    join groups in _context.Groups on subjects.GroupId equals groups.Id
                                    join users in _context.Users on us.UserId equals users.Id
                                    where subjects.LessonId.Equals(model.LessonId) && subjects.Week.Equals(info.Week)
                                    && subjects.TeacherId.Equals(model.TeacherId)
                                    select new LessonHelpDto
                                    {
                                        GroupId = subjects.GroupId,
                                        StudentId = us.UserId,
                                        MarkTime = us.MarkTime.ToString(),
                                        GroupName = groups.Name,
                                        StudentName = users.LastName + " " + users.FirstName[0] + ". " + users.MiddleName[0] + ".",
                                    })
                                    .ToListAsync(cancellationToken);

                // можно вытащить имена учеников, их группы, время отметки
                var count2 = query2.Count;

                double tmp = Math.Round((double)count2 / count * 100, 2);
                // возвращаем процент
                return new LessonsStatisticDto
                {
                    Students = query2,
                    Perсentage = tmp
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Ошибка получения статистики для занятия учителя {model.TeacherId} с идентификатором {model.LessonId}");
                throw;
            }
        }

        private static Lesson CreateModel(LessonDto model, Lesson lesson)
        {
            lesson.Name = model.Name;
            return lesson;
        }

        private static LessonResponseDto CreateModel(Lesson lesson)
        {
            return new LessonResponseDto
            {
                Id = lesson.Id,
                Name = lesson.Name,
                Subjects = lesson.Subjects?.Select(x => x.Id).ToList(),
            };
        }
    }
}
