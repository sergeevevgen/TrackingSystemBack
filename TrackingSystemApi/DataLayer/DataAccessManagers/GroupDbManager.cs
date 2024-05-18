using TrackingSystem.Api.DataLayer.Data;
using TrackingSystem.Api.DataLayer.Models;
using TrackingSystem.Api.Shared.Dto.Group;
using TrackingSystem.Api.Shared.IManagers.DbManagers;
using Microsoft.EntityFrameworkCore;

namespace TrackingSystem.Api.DataLayer.DataAccessManagers
{
    /// <summary>
    /// Класс для взаимодействия с сущностью "Группа"
    /// </summary>
    public class GroupDbManager : IGroupDbManager
    {
        private readonly ILogger _logger;
        private readonly TrackingSystemContext _context;

        public GroupDbManager(
            ILogger logger,
            TrackingSystemContext context)
        {
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// Метод для удаления группы
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> Delete(GroupDto model, CancellationToken cancellationToken = default)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                Group element = await _context.Groups
                    .FirstOrDefaultAsync(g => g.Id.Equals(model.Id.Value) 
                    || g.Name.Equals(model.Name), cancellationToken);

                if (element != null)
                {
                    _context.Groups.Remove(element);

                    await _context.SaveChangesAsync(cancellationToken);
                    await transaction.CommitAsync(cancellationToken);

                    return true;
                }

                throw new Exception("Элемент не найден");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.Error(ex, $"Ошибка удаления группы c Id {model.Id}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Метод для получения группы по идентификатору или названию
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<GroupResponseDto?> GetElement(GroupDto model, CancellationToken cancellationToken = default)
        {
            try
            {
                var query = _context.Groups
                    .Include(g => g.Subjects)
                    .Include(g => g.Users)
                    .AsNoTracking()
                    .AsQueryable();

                if (model.Id.HasValue)
                {
                    query = query.Where(g => g.Id == model.Id.Value);
                }
                else if (!string.IsNullOrEmpty(model.Name))
                {
                    query = query.Where(g => g.Name.Equals(model.Name));
                }
                else
                {
                    return null;
                }

                var element = await query.FirstOrDefaultAsync(cancellationToken);

                return element == null ? null : CreateModel(element);
            }
            catch(Exception ex)
            {
                _logger.Error(ex, $"Ошибка получения группы c Id {model.Id}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Метод для создания группы
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<GroupResponseDto> Insert(GroupDto model, CancellationToken cancellationToken = default)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                Group group = new();

                await _context.Groups.AddAsync(CreateModel(model, group));
                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return CreateModel(group);
            }
            catch(Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.Error(ex, $"Ошибка создания группы: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Метод для обновления группы
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<GroupResponseDto> Update(GroupDto model, CancellationToken cancellationToken = default)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var element = await _context.Groups
                    .FirstOrDefaultAsync(g => g.Id.Equals(model.Id.Value), cancellationToken) ?? throw new Exception($"Группа с Id {model.Id} не найдена");

                CreateModel(model, element);
                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return CreateModel(element);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.Error(ex, $"Ошибка обновления группы c Id {model.Id}: {ex.Message}");
                throw;
            }
        }

        private static Group CreateModel(GroupDto model, Group group)
        {
            group.Name = model.Name;
            return group;
        }

        private static GroupResponseDto CreateModel(Group group)
        {
            return new GroupResponseDto
            {
                Id = group.Id,
                Name = group.Name,
                Users = group.Users.Select(x => x.Id).ToList(),
                Subjects = group.Subjects.Select(x => x.Id).ToList(),
            };
        }
    }
}
