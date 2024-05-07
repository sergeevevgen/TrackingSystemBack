using TrackingSystem.Api.DataLayer.Data;
using TrackingSystem.Api.Shared.Dto.Place;
using TrackingSystem.Api.Shared.IManagers.DbManagers;
using Microsoft.EntityFrameworkCore;
using TrackingSystem.Api.DataLayer.Models;

namespace TrackingSystem.Api.DataLayer.DataAccessManagers
{
    public class PlaceDbManager : IPlaceDbManager
    {
        private readonly ILogger _logger;
        private readonly TrackingSystemContext _context;

        public PlaceDbManager(
            ILogger logger,
            TrackingSystemContext context)
        {
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// Метод для удаления помещения
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task Delete(PlaceDto model, CancellationToken cancellationToken)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                Place element = await _context.Places
                    .FirstOrDefaultAsync(g => g.Id.Equals(model.Id.Value)
                    || g.Name.Equals(model.Name), cancellationToken);

                if (element != null)
                {
                    _context.Places.Remove(element);
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
                _logger.Error(ex, $"Ошибка удаления помещения c Id {model.Id}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Метод для получения помещения по идентификатору или названию
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<PlaceResponseDto?> GetElement(PlaceDto model, CancellationToken cancellationToken)
        {
            try
            {
                var element = await _context.Places
                    .Include(g => g.Subjects)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(g => g.Id.Equals(model.Id.Value)
                    || g.Name.Equals(model.Name), cancellationToken);

                return element == null ? null : CreateModel(element);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Ошибка получения помещения c Id {model.Id}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Метод для создания помещения
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task Insert(PlaceDto model, CancellationToken cancellationToken)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                await _context.Places.AddAsync(CreateModel(model, new Place()));
                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.Error(ex, $"Ошибка создания помещения: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Метод для обновления помещения
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task Update(PlaceDto model, CancellationToken cancellationToken)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var element = await _context.Places
                    .FirstOrDefaultAsync(g => g.Id.Equals(model.Id.Value), cancellationToken);

                if (element == null)
                {
                    throw new Exception($"Помещение с Id {model.Id} не найдена");
                }

                CreateModel(model, element);
                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.Error(ex, $"Ошибка обновления помещения c Id {model.Id}: {ex.Message}");
                throw;
            }
        }

        private static Place CreateModel(PlaceDto model, Place place)
        {
            place.Name = model.Name;
            return place;
        }

        private static PlaceResponseDto CreateModel(Place place)
        {
            return new PlaceResponseDto
            {
                Id = place.Id,
                Name = place.Name,
                Subjects = place.Subjects.Select(x => x.Id).ToList(),
            };
        }
    }
}
