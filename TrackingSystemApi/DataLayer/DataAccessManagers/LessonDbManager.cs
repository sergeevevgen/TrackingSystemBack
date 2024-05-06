using TrackingSystem.Api.Shared.Dto.Discipline;
using TrackingSystem.Api.Shared.IManagers.DbManagers;
using Microsoft.EntityFrameworkCore;
using TrackingSystem.Api.DataLayer.Data;
using TrackingSystem.Api.DataLayer.Models;

namespace TrackingSystem.Api.DataLayer.DataAccessManagers
{
    public class LessonDbManager : ILessonDbManager
    {
        private readonly ILogger _logger;

        public LessonDbManager(
            ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Метод для удаления типа занятия
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task Delete(LessonDto model, CancellationToken cancellationToken)
        {
            using var context = new TrackingSystemContext();
            using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                Lesson element = await context.Lessons
                    .FirstOrDefaultAsync(g => g.Id.Equals(model.Id.Value)
                    || g.Name.Equals(model.Name), cancellationToken);

                if (element != null)
                {
                    context.Lessons.Remove(element);
                    await context.SaveChangesAsync(cancellationToken);
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
        public async Task<LessonResponseDto?> GetElement(LessonDto model, CancellationToken cancellationToken)
        {
            using var context = new TrackingSystemContext();
            try
            {
                var element = await context.Lessons
                    .Include(g => g.Subjects)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(g => g.Id.Equals(model.Id.Value)
                    || g.Name.Equals(model.Name), cancellationToken);

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
        public async Task Insert(LessonDto model, CancellationToken cancellationToken)
        {
            using var context = new TrackingSystemContext();
            using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                await context.Lessons.AddAsync(CreateModel(model, new Lesson()));
                await context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
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
        public async Task Update(LessonDto model, CancellationToken cancellationToken)
        {
            using var context = new TrackingSystemContext();
            using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var element = await context.Lessons
                    .FirstOrDefaultAsync(g => g.Id.Equals(model.Id.Value), cancellationToken);

                if (element == null)
                {
                    throw new Exception($"Тип задания с Id {model.Id} не найден");
                }

                CreateModel(model, element);
                await context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
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
                Subjects = lesson.Subjects.Select(x => x.Id).ToList(),
            };
        }
    }
}
