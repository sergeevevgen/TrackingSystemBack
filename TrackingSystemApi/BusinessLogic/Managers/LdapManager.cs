using Microsoft.Extensions.Options;
using System.DirectoryServices.Protocols;
using System.Net;
using TrackingSystem.Api.AppLogic.Core;
using TrackingSystem.Api.Shared.Dto.User;
using TrackingSystem.Api.Shared.Enums;
using TrackingSystem.Api.Shared.IManagers.LogicManagers;

namespace TrackingSystem.Api.BusinessLogic.Managers
{
    public class LdapManager : ILdapDownloadManager
    {
        private readonly ILogger _logger;
        private readonly AppConfig _appConfig;
        private readonly IUserManager _manager;
        // Базовая строка поиска в LDAP
        private readonly string searchBase = "ou=accounts,dc=ams,dc=ulstu,dc=ru";

        public LdapManager(
            ILogger logger, 
            IOptions<AppConfig> options,
            IUserManager manager)
        {
            _logger = logger;
            _appConfig = options.Value;
            _manager = manager;
        }

        /// <summary>
        /// Метод для синхронизации с Ldap
        /// </summary>
        /// <returns></returns>
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

                // Главный лист с пользователями
                var mainList = new List<UserLdapDto>();

                // Лист с аккаунтами
                var listAccounts = SearchForUserAccounts(cn);

                // Лист с инфой об учёбе
                var listCourse = SearchForCourses(cn);

                // Лист с инфой о работе
                var listJob = SearchForJobs(cn);

                // Добавляем элементы в главный лист
                foreach(var element in listAccounts)
                {
                    mainList.Add(new UserLdapDto
                    {
                        UserLogin = element.UID,
                        FirstName = element.FirstName,
                        LastName = element.LastName,
                        MiddleName = element.MiddleName
                    });
                }

                // Вытаскиваем элементы из листа с учёбой
                var courseList = new List<UserGroupLdapDto>();
                foreach(var element in listCourse)
                {
                    courseList.Add(element);
                }

                // Вытаскиваем элементы из листа с работой
                var jobList = new List<UserJobLdapDto>();
                foreach(var element in listJob)
                {
                    jobList.Add(element);
                }

                // Сливаем их вместе
                foreach (var c in courseList)
                {
                    var user = mainList.FirstOrDefault(e => e.UserLogin.Equals(c.UID));

                    if (user != null)
                    {
                        user.Role = Role.Pupil;
                        user.Group = !string.IsNullOrEmpty(c.GroupName) ? c.GroupName : null;
                        user.Status = !string.IsNullOrEmpty(c.GroupName) ? Status.Studying : Status.Dropped;
                    }
                }

                foreach (var job in jobList)
                {
                    var user = mainList.FirstOrDefault(e => e.UserLogin.Equals(job.UID));

                    if (user != null)
                    {
                        user.Role = Role.Teacher;
                        user.Group = null;
                        user.Status = null;
                    }
                }

                foreach (var user in mainList)
                {
                    await _manager.CreateOrUpdateFromLdap(user);
                }

                cn.Dispose();
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
            string[] attributes = { "cn", "uid", "firstName", "lastName", "middleName" };
            var req = new SearchRequest(searchBase, filter, SearchScope.Subtree, attributes);
            var resp = (SearchResponse)cn.SendRequest(req);

            foreach (SearchResultEntry entry in resp.Entries)
            {
                var user = new UserFromLdapDto
                {
                    CN = GetStringAttribute(entry, "cn"),
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
                    UID = entry.DistinguishedName.Split(",")[1].Split("=")[1],
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
            string filter = "(objectClass=ulstuJob)";
            string[] attributes = { "jobTitle", "jobStake", "employmentType", "jobType" };
            var req = new SearchRequest(searchBase, filter, SearchScope.Subtree, attributes);
            var resp = (SearchResponse)cn.SendRequest(req);

            foreach (SearchResultEntry entry in resp.Entries)
            {
                var user = new UserJobLdapDto
                {
                    UID = entry.DistinguishedName.Split(",")[1].Split("=")[1],
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
