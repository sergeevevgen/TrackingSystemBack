using Microsoft.Extensions.Options;
using System.DirectoryServices.Protocols;
using System.Net;
using TrackingSystem.Api.AppLogic.Core;
using TrackingSystem.Api.Shared.Dto.User;

namespace TrackingSystem.Api.BusinessLogic.Managers
{
    public class LdapManager
    {
        private readonly ILogger _logger;
        private readonly AppConfig _appConfig;

        // Базовая строка поиска в LDAP
        private readonly string searchBase = "ou=accounts,dc=ams,dc=ulstu,dc=ru";

        public LdapManager(ILogger logger, IOptions<AppConfig> options)
        {
            _logger = logger;
            _appConfig = options.Value;
        }

        public async Task SynchWithLdap()
        {
            _logger.Info("Начало синхронизации с LDAP");
            try { 
                // Подключение к серверу LDAP
                var server = new LdapDirectoryIdentifier(_appConfig.LdapHost, _appConfig.LdapPort);

                // Креды для доступа к серверу
                var credentials = new NetworkCredential(_appConfig.LdapLoginDn, _appConfig.LdapPassword);

                // Создаем подключение к серверу LDAP
                var cn = new LdapConnection(server);
                cn.SessionOptions.ProtocolVersion = 3;
                cn.AuthType = AuthType.Basic;
                cn.Bind(credentials);

                
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Возникла ошибка в процессе синхронизации с LDAP {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Метод для вытаскивания аккаунтов пользователей из LDAP
        /// </summary>
        /// <param name="cn"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        private IEnumerable<UserFromLdapDto> SearchForUserAccounts(LdapConnection cn)
        {
            string filter = "(&(objectClass=ulstuPerson)(accountStatus=active)(!(iduniv=SYSTEMACC)))";
            string[] attributes = { "cn", "userPassword", "uid", "firstName", "lastName", "middleName" };
            var req = new SearchRequest(searchBase, filter, SearchScope.Subtree, attributes);
            var resp = (SearchResponse)cn.SendRequest(req);

            foreach (SearchResultEntry entry in resp.Entries)
            {
                var user = new UserFromLdapDto
                {
                    CN = GetStringAttribute(entry, "cn"),
                    Password = GetStringAttribute(entry, "userPassword"),
                    UID = GetStringAttribute(entry, "uid"),
                    FirstName = GetStringAttribute(entry, "firstName"),
                    LastName = GetStringAttribute(entry, "lastName"),
                    MiddleName = GetStringAttribute(entry, "middleName")
                };
                yield return user;
            }
            yield break;
        }

        /// <summary>
        /// Метод для вытаскивания информации по обучению пользователя из LDAP
        /// </summary>
        /// <param name="cn"></param>
        /// <param name="filter"></param>
        /// <param name="searchBase"></param>
        /// <returns></returns>
        private IEnumerable<UserGroupLdapDto> SearchForCourses(LdapConnection cn)
        {
            string filter = "(objectClass=ulstuCourse)";
            string[] attributes = { "course", "currentState", "faculty", "groupName", "specialty" };
            var req = new SearchRequest(searchBase, filter, SearchScope.Subtree, attributes);
            var resp = (SearchResponse)cn.SendRequest(req);

            foreach (SearchResultEntry entry in resp.Entries)
            {
                var user = new UserGroupLdapDto
                {
                    CN = entry.DistinguishedName, // TODO - надо вытаскивать логин
                    Course = GetStringAttribute(entry, "course"),
                    CurrentState = GetStringAttribute(entry, "currentState"),
                    Faculty = GetStringAttribute(entry, "faculty"),
                    GroupName = GetStringAttribute(entry, "groupName"),
                    Specialty = GetStringAttribute(entry, "specialty")
                };
                yield return user;
            }
            yield break;
        }

        /// <summary>
        /// Метод для вытаскивания информации по работе пользователя из LDAP
        /// </summary>
        /// <param name="cn"></param>
        /// <param name="filter"></param>
        /// <param name="searchBase"></param>
        /// <returns></returns>
        private IEnumerable<UserJobLdapDto> SearchForJobs(LdapConnection cn)
        {
            string[] attributes = { "jobTitle", "jobStake", "employmentType", "jobType" };
            var req = new SearchRequest(searchBase, filter, SearchScope.Subtree, attributes);
            var resp = (SearchResponse)cn.SendRequest(req);

            foreach (SearchResultEntry entry in resp.Entries)
            {
                var user = new UserJobLdapDto
                {
                    CN = entry.DistinguishedName, // TODO - надо вытаскивать логин
                    JobTitle = GetStringAttribute(entry, "jobTitle"),
                    JobStake = GetStringAttribute(entry, "jobStake"),
                    EmploymentType = GetStringAttribute(entry, "employmentType"),
                    JobType = GetStringAttribute(entry, "jobType")
                };
                yield return user;
            }
            yield break;
        }

        private static string GetStringAttribute(SearchResultEntry entry, string key)
        {
            if (!entry.Attributes.Contains(key))
            {
                return string.Empty;
            }
            string[] rawVal = (string[])entry.Attributes[key].GetValues(typeof(string));
            return rawVal[0];
        }
    }
}
