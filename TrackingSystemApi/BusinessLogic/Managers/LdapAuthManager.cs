using Microsoft.Extensions.Options;
using System.DirectoryServices.Protocols;
using System.Net;
using TrackingSystem.Api.AppLogic.Core;
using TrackingSystem.Api.Shared.Dto.User;
using TrackingSystem.Api.Shared.IManagers.LogicManagers;

namespace TrackingSystem.Api.BusinessLogic.Managers
{
    public class LdapAuthManager : ILdapAuthManager
    {
        private readonly ILogger _logger;
        private readonly AppConfig _appConfig;
        // Базовая строка поиска в LDAP
        private readonly string searchBase = "ou=accounts,dc=ams,dc=ulstu,dc=ru";

        public LdapAuthManager(ILogger logger,
            IOptions<AppConfig> options)
        {
            _logger = logger;
            _appConfig = options.Value;
        }


        /// <summary>
        /// Метод для авторизации в LDAP
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public bool CanAuthorize(UserLoginDto dto)
        {
            try
            {
                // Подключение к серверу LDAP
                var server = new LdapDirectoryIdentifier(_appConfig.LdapHost, _appConfig.LdapPort);

                // Креды для доступа к серверу
                var credentials = new NetworkCredential($"uid={dto.Login},{searchBase}", dto.Password);

                // Создаем подключение к серверу LDAP
                var cn = new LdapConnection(server);
                cn.SessionOptions.ProtocolVersion = 3;
                cn.AuthType = AuthType.Basic;
                cn.Bind(credentials);

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Возникла ошибка в процессе авторизации с LDAP {ex.Message}");
                throw;
            }
        }
    }
}
