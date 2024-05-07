using TrackingSystem.Api.DataLayer.Data;
using TrackingSystem.Api.Shared.Dto.Subject;
using TrackingSystem.Api.Shared.IManagers.DbManagers;
using Microsoft.EntityFrameworkCore;
using TrackingSystem.Api.DataLayer.Models;

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
        public async Task Delete(SubjectDto model, CancellationToken cancellationToken)
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
        public async Task<SubjectResponseDto?> GetElement(SubjectDto model, CancellationToken cancellationToken)
        {
            try
            {
                // Ищем пользователя сначала по логину, потом по идентификатору
                var element = await _context.Subjects
                    .Include(s => s.Group)
                    .Include(s => s.Lesson)
                    .Include(s => s.Place)
                    .Include(s => s.Users)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.Id.Equals(model.Id.Value), cancellationToken);

                return element == null ? null : CreateModel(element);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Ошибка получения пользователя c Id {model.Id}: {ex.Message}");
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
        public async Task Insert(SubjectDto model, CancellationToken cancellationToken)
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
        public async Task Update(SubjectDto model, CancellationToken cancellationToken)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var element = await _context.Subjects
                    .FirstOrDefaultAsync(u => u.Id.Equals(model.Id), cancellationToken);

                if (element == null)
                {
                    throw new Exception($"Занятие с Id {model.Id} не найдено");
                }

                await CreateModel(model, element, _context);
                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.Error(ex, $"Ошибка обновления занятия c Id {model.Id}: {ex.Message}");
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
    }
}
