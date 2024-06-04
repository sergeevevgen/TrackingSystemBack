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
