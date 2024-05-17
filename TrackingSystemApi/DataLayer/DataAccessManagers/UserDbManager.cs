using NLog;
using Microsoft.EntityFrameworkCore;
using TrackingSystem.Api.DataLayer.Data;
using TrackingSystem.Api.Shared.Dto.User;
using TrackingSystem.Api.Shared.Enums;
using TrackingSystem.Api.Shared.IManagers.DbManagers;
using TrackingSystem.Api.DataLayer.Models;
using AngleSharp.Dom;

namespace TrackingSystem.Api.DataLayer.DataAccessManagers
{
    /// <summary>
    /// Класс для взаимодействия с сущностью "Пользователь"
    /// </summary>
    public class UserDbManager : IUserDbManager
    {
        private readonly ILogger _logger;
        private readonly TrackingSystemContext _context;

        public UserDbManager(
            ILogger logger,
            TrackingSystemContext context)
        {
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// Метод для поиска пользователя
        /// </summary>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<UserResponseDto> FindUser(UserFindDto query, CancellationToken cancellationToken = default)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                UserResponseDto? result = null;

                // Фильтр по логину
                if (!string.IsNullOrEmpty(query.Login) && !string.IsNullOrEmpty(query.Password))
                {
                    var login = query.Login.ToLower();

                    result = await _context.Users
                           .Where(
                            u => u.Login.ToLower() == login) //&&
                            //u.Password == query.Password)
                           .Select(u => new UserResponseDto
                           {
                               Id = u.Id,
                               Login = u.Login,
                               Role = u.Role,
                               Name = u.LastName + " " + u.FirstName[0] + ". " + u.MiddleName + ".",
                               Group = u.UserGroup.Name,
                               Status = u.Status,
                               GroupId = u.GroupId,
                           })
                           .AsNoTracking()
                           .FirstOrDefaultAsync(cancellationToken);
                }

                // Фильтр по Id
                if (query.Id.HasValue)
                {
                    
                    result = await _context.Users
                           .Where(u => u.Id == query.Id)
                           .Select(u => new UserResponseDto
                           {
                               Id = u.Id,
                               Login = u.Login,
                               Role = u.Role,
                               Name = u.LastName + " " + u.FirstName[0] + ". " + u.MiddleName + ".",
                               Group = u.UserGroup.Name,
                               Status = u.Status,
                               GroupId = u.GroupId,
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
        public async Task<UserFindResponseDto?> FindUserById(UserFindDto request, CancellationToken cancellationToken = default)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                return await _context.Users
                                .Where(u => u.Id == request.Id)
                                .Select(u => new UserFindResponseDto
                                {
                                    Login = u.Login,
                                    Name = u.LastName + " " + u.FirstName[0] + ". " + u.MiddleName + ".",
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
        public async Task<UserResponseDto> Insert(UserDto model, CancellationToken cancellationToken = default)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                User user = new();

                await _context.Users.AddAsync(CreateModel(model, user));

                await _context.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                return CreateModel(user);
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
        public async Task<UserResponseDto> Update(UserDto model, CancellationToken cancellationToken = default)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var element = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id.Equals(model.Id),
                    cancellationToken) ?? throw new Exception($"Пользователь с Id {model.Id} не найден");

                CreateModel(model, element);
                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return CreateModel(element);
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
        public async Task<bool> Delete(UserDto model, CancellationToken cancellationToken = default)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                User element = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id.Equals(model.Id.Value), 
                    cancellationToken);

                if (element != null)
                {
                    _context.Users.Remove(element);
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
        public async Task<UserResponseDto?> GetElement(UserDto model, CancellationToken cancellationToken = default)
        {
            try
            {
                // Ищем пользователя сначала по логину, потом по идентификатору
                var element = await _context.Users
                    .Include(u => u.UserGroup)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => 
                        u.Login.Equals(model.Login) || 
                        u.Id.Equals(model.Id.Value) || 
                        u.LastName.Contains(model.LastName), cancellationToken);

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
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.MiddleName = model.MiddleName;
            user.Login = model.Login;
            user.Password = model.Password;
            user.GroupId = model.GroupId;
            user.Status = model.Status;
            user.Role = model.Role;

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
                Name = user.LastName + " " + user.FirstName[0] + ". " + user.MiddleName[0] + ".",
                GroupId = user.GroupId,
                Group = user.UserGroup?.Name,
                Status = user.Status,
                Role = user.Role,
            };
        }
    }
}
