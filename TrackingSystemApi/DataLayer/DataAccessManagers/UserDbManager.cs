using NLog;
using Microsoft.EntityFrameworkCore;
using TrackingSystem.Api.DataLayer.Data;
using ILogger = NLog.ILogger;
using TrackingSystem.Api.Shared.Dto.User;
using TrackingSystem.Api.Shared.Enums;
using TrackingSystem.Api.Shared.IManagers;
using TrackingSystem.Api.DataLayer.Models;

namespace TrackingSystem.Api.DataLayer.DataAccessManagers
{
    /// <summary>
    /// Класс для взаимодействия с сущностью "Пользователь"
    /// </summary>
    public class UserDbManager : IUserDbManager
    {
        private readonly ILogger _logger;

        public UserDbManager(
            ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Метод для поиска пользователя
        /// </summary>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<UserResponseDto> FindUser(UserFindDto query, CancellationToken cancellationToken)
        {
            using var context = new TrackingSystemContext();
            using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                UserResponseDto? result = null;

                // Фильтр по логину
                if (!string.IsNullOrEmpty(query.Login) && !string.IsNullOrEmpty(query.Password))
                {
                    var login = query.Login.ToLower();

                    result = await context.Users
                           .Where(
                            u => u.Login.ToLower() == login &&
                            u.Password == query.Password)
                           .Select(u => new UserResponseDto
                           {
                               Id = u.Id,
                               Login = u.Login,
                               Roles = u.Roles.Select(t => t.Role.Name).ToList(), // TODO
                               Name = u.Name,
                               Group = u.UserGroup.Name,
                               Status = u.Status,
                           })
                           .AsNoTracking()
                           .FirstOrDefaultAsync(cancellationToken);
                }

                // Фильтр по Id
                if (query.Id.HasValue)
                {
                    
                    result = await context.Users
                           .Where(u => u.Id == query.Id)
                           .Select(u => new UserResponseDto
                           {
                               Id = u.Id,
                               Login = u.Login,
                               Roles = u.Roles.Select(t => t.Role.Name).ToList(), // TODO
                               Name = u.Name,
                               Group = u.UserGroup.Name,
                               Status = u.Status,
                           })
                           .AsNoTracking()
                           .FirstOrDefaultAsync(cancellationToken);
                }

                return result ?? throw new Exception("Были введены неверные данные пользователя");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.Error(ex, "Ошибка получения пользователя по запросу");
                throw;
            }
        }

        /// <summary>
        /// Поиск пользователя по ID
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<UserFindResponseDto?> FindUserById(UserFindDto request, CancellationToken cancellationToken)
        {
            using var context = new TrackingSystemContext();
            using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                return await context.Users
                                .Where(u => u.Id == request.Id)
                                .Select(u => new UserFindResponseDto
                                {
                                    Login = u.Login,
                                    Name = u.Name,
                                })
                                .AsNoTracking()
                                .FirstOrDefaultAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.Error(ex, "Ошибка получения пользователя по идентификатору");
                throw;
            }
        }

        /// <summary>
        /// Метод для создания пользователя (Пользователь создается самостоятельно, потом к нему уже привязываются основные сущности. Но изначально должны быть роли)
        /// </summary>
        /// <returns></returns>
        public async Task Insert(UserDto model, CancellationToken cancellationToken)
        {
            
            using var context = new TrackingSystemContext();
            using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                await context.Users.AddAsync(CreateModel(model, new User()), cancellationToken);
                await context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.Error(ex, $"Ошибка создания пользователя: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Метод для обновления полей пользователя
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task Update(UserDto model, CancellationToken cancellationToken)
        {
            using var context = new TrackingSystemContext();
            using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var element = await context.Users
                    .FirstOrDefaultAsync(u => u.Id.Equals(model.Id), cancellationToken);

                if (element == null)
                {
                    throw new Exception($"Пользователь с Id {model.Id} не найден");
                }

                CreateModel(model, element);
                await context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.Error(ex, $"Ошибка обновления пользователя c Id {model.Id}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Метод для удаления пользователя
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task Delete(UserDto model, CancellationToken cancellationToken)
        {
            using var context = new TrackingSystemContext();
            using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                User element = await context.Users
                    .FirstOrDefaultAsync(u => u.Id.Equals(model.Id.HasValue), cancellationToken);

                if (element != null)
                {
                    context.Users.Remove(element);
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
                _logger.Error(ex, $"Ошибка удаления пользователя c Id {model.Id}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Метод для получения пользователя (проверить, правильно ли настроены Include, иначе в responseModel будут возвращаться нуллы)
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<UserResponseDto?> GetElement(UserDto model, CancellationToken cancellationToken)
        {
            using var context = new TrackingSystemContext();
            try
            {
                // Ищем пользователя сначала по Логину, потом по идентификатору
                var element = await context.Users
                    .Include(u => u.UserGroup)
                    .Include(u => u.Roles)
                    .ThenInclude(ur => ur.Role)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Login.Equals(model.Login) || u.Id.Equals(model.Id), cancellationToken);

                return element == null ? null : CreateModel(element);
                    
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Ошибка получения пользователя c Id {model.Id}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Метод для биндинга полей dto и модели db
        /// </summary>
        /// <param name="model"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        private static User CreateModel(UserDto model, User user)
        {
            user.GroupId = model.GroupId;
            user.Name = model.Name;
            user.Password = model.Password;
            user.Status = model.Status;
            user.Login = model.Login;
            return user;
        }

        /// <summary>
        /// Метод для биндинга полей модели db и полей dto
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private static UserResponseDto CreateModel(User user)
        {
            return new UserResponseDto
            {
                Id = user.Id,
                Login = user.Login,
                Name = user.Name,
                GroupId = user.GroupId,
                Group = user.UserGroup.Name,
                Status = user.Status,
                Roles = user.Roles.Select(r => r.Role.Name).ToList(),
            };
        }
    }
}
