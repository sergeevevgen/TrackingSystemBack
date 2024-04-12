using NLog;
using Microsoft.EntityFrameworkCore;
using TrackingSystem.Api.DataLayer.Data;
using ILogger = NLog.ILogger;
using TrackingSystem.Api.Shared.Dto.User;
using TrackingSystem.Api.Shared.Enums;

namespace TrackingSystem.Api.DataLayer.DataAccessManagers
{
    public class UserDbManager
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

        public async Task<UserResponseData> FindUser(UserDataQuery query, CancellationToken cancellationToken)
        {
            try
            {
                UserResponseData? result = null;

                // Фильтр по логину
                if (!string.IsNullOrEmpty(query.Login) && !string.IsNullOrEmpty(query.Password))
                {
                    var login = query.Login.ToLower();

                    result = await _context.Users
                           .Where(
                            u => u.Login.ToLower() == login &&
                            u.Password == query.Password)
                           .Select(u => new UserResponseData
                           {
                               Id = u.Id,
                               Login = u.Login,
                               Roles = new List<ERoles> { ERoles.Pupil, ERoles.Teacher }, // TODO
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
                    result = await _context.Users
                           .Where(u => u.Id == query.Id)
                           .Select(u => new UserResponseData
                           {
                               Id = u.Id,
                               Login = u.Login,
                               Roles = new List<ERoles> { ERoles.Pupil, ERoles.Teacher }, // TODO
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
                _logger.Error(ex, "Ошибка получения пользователя по запросу");
                throw;
            }
        }

        public async Task<UserByIdResponse?> FindUserById(UserByIdQuery request, CancellationToken cancellationToken)
        {
            return await _context.Users
                            .Where(u => u.Id == request.UserId)
                            .Select(u => new UserByIdResponse
                            {
                                Login = u.Login,
                                Name = u.Name,
                            })
                            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
        }
    }
}
