using TrackingSystem.Api.DataLayer.Data;
using TrackingSystem.Api.Shared.Enums;
using TrackingSystem.Api.Shared.IManagers.DbManagers;

namespace TrackingSystem.Api.DataLayer.DataAccessManagers
{
    public class RoleDbManager : IRoleDbManager
    {
        private readonly ILogger _logger;
        private readonly TrackingSystemContext _context;

        public RoleDbManager(
            ILogger logger,
            TrackingSystemContext context)
        {
            _logger = logger;
            _context = context;
        }

        public Guid? GetElement(ERoles role)
        {
            try
            {
                // Ищем пользователя сначала по логину, потом по идентификатору

                // Если не по идентификатору, то по всем остальным полям
                var element = _context.Roles
                    .FirstOrDefault(el => el.Name.Equals(role));

                return element?.Id;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Ошибка получения занятия c Id {role}: {ex.Message}");
                throw;
            }
        }
    }
}
