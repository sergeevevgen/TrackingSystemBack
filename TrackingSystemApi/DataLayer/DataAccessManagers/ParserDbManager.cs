using Microsoft.EntityFrameworkCore;
using TrackingSystem.Api.DataLayer.Data;
using TrackingSystem.Api.Shared.Dto.User;
using TrackingSystem.Api.Shared.IManagers;
using ILogger = NLog.ILogger;

namespace TrackingSystem.Api.DataLayer.DataAccessManagers
{
    public class ParserDbManager : IParserDbManager
    {
        private readonly ILogger _logger;
        private readonly TrackingSystemContext _context;

        public ParserDbManager(
            ILogger logger,
            TrackingSystemContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<bool> DeleteSubjectIsDifferenceZero()
        {
            try 
            {
                // Получаем лист занятий, которые надо удалить
                var listToDelete = _context.Subjects
                    .Where(s => s.IsDifference == 0);

                // Удаляем
                _context.Subjects.RemoveRange(listToDelete);

                // Обновляем статус записей
                await _context.Database.ExecuteSqlRawAsync("Update Subject SET isdifference = 0");

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Ошибка получения пользователя по идентификатору");
                throw;
            }
        }

        public async Task<UserResponseData> GetTeacher(UserCreateQuery user) 
        {
            try
            {
                var u = await _context.Users
                    .FirstOrDefaultAsync(u => u.Name.Equals(user.Name));

                return u != null ? u.Id : Guid.Empty;

            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                return Guid.Empty;
            }
        }

        public async Task<Guid> CreateTeacher(UserCreateQuery user)
        {
            try
            {
                var u = 
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                return Guid.Empty;
            }
        }
    }
}
