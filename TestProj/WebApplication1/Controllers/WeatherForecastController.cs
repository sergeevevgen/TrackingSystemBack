using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Security.Cryptography;
//using Novell.Directory.Ldap;
using System.DirectoryServices.Protocols;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        //private LdapConnection connection;

        [HttpGet(Name = "Test2")]
        public async Task<string> Test2()
        {
            TestFuncLdap();
            return "Ok";
        }

        private void TestFuncLdap()
        {
            string ldapHost = "lk.ustu";
            int ldapPort = 389;
            string loginDN = "cn=attendance,ou=services,dc=ams,dc=ulstu,dc=ru";
            string password = "YaLeKm+8ER+7&m&4&DA";

            var server = new LdapDirectoryIdentifier(ldapHost, ldapPort);
            var credentials = new NetworkCredential(loginDN, password);

            var cn = new LdapConnection(server);
            cn.SessionOptions.ProtocolVersion = 3;
            cn.AuthType = AuthType.Basic;
            cn.Bind(credentials);

            string filter = "(&(objectClass=ulstuPerson)(accountStatus=active)(!(iduniv=SYSTEMACC)))";
            string searchBase = "ou=accounts,dc=ams,dc=ulstu,dc=ru";
            Search(cn, filter, searchBase);
        }

        private IEnumerable<object> Search(LdapConnection cn, string filter, string searchBase = "")
        {
            string[] attributes = { "cn", "userPassword", "uid", "lastName" };
            var req = new SearchRequest(searchBase, filter, SearchScope.Subtree, attributes);
            var resp = (SearchResponse)cn.SendRequest(req);

            foreach (SearchResultEntry entry in resp.Entries)
            {
                var user = new
                {
                    Name = GetStringAttribute(entry, "cn"),
                    Mail = GetStringAttribute(entry, "userPassword"),
                    Company = GetStringAttribute(entry, "uid"),
                    Location = GetStringAttribute(entry, "lastName")
                };
                yield return user;
            }
            yield break;
        }

        private string GetStringAttribute(SearchResultEntry entry, string key)
        {
            if (!entry.Attributes.Contains(key))
            {
                return string.Empty;
            }
            string[] rawVal = (string[])entry.Attributes[key].GetValues(typeof(string));
            return rawVal[0];
        }

        // Можно, конечно, сделать сначала общую выгрузку по ulstuPerson, а затем уже по логину проходится по ulstuJob и ulstuPerson
        //private void TestFuncLdap()
        //{
        //    string ldapHost = "lk.ustu";
        //    int ldapPort = 389;
        //    string loginDN = "cn=attendance,ou=services,dc=ams,dc=ulstu,dc=ru";
        //    string password = "YaLeKm+8ER+7&m&4&DA";

        //    string searchBase = "ou=accounts,dc=ams,dc=ulstu,dc=ru";
        //    //string searchFilter = "(&(|(objectClass=ulstuPerson)(objectClass=ulstuJob)(objectClass=ulstuCourse))(accountStatus=active)(!(iduniv=SYSTEMACC)))";
        //    //string searchFilter = "(|(objectClass=ulstuJob)(objectClass=ulstuCourse))";
        //    string searchFilter = "(objectClass=ulstuCourse)";
        //    int ldapVersion = LdapConnection.LdapV3;
        //    try
        //    {

        //        LdapConnection conn = new LdapConnection();

        //        conn.Connect(ldapHost, ldapPort);
        //        conn.Bind(ldapVersion, loginDN, password);

        //        string[] requiredAttributes = { "groupName", "course", "faculty", "entryDN" };
        //        //ILdapSearchResults lsc = conn.Search(searchBase,
        //        //                    LdapConnection.ScopeSub,
        //        //                    searchFilter,
        //        //                    requiredAttributes,
        //        //                    false);
        //        ILdapSearchResults lsc = conn.Search(searchBase,
        //                            LdapConnection.ScopeSub,
        //                            searchFilter,
        //                            null,
        //                            false);

        //        while (lsc.HasMore())
        //        {
        //            LdapEntry nextEntry = null;
        //            try
        //            {
        //                nextEntry = lsc.Next();
        //                if (!nextEntry.Dn.Contains("e.sergeev"))
        //                {
        //                    continue;
        //                }
        //            }
        //            catch (LdapException e)
        //            {
        //                // Console.WriteLine("Error : " + e.LdapErrorMessage);
        //                continue;
        //            }
        //            // Console.WriteLine("\n" + nextEntry.Dn);

        //            // Атрибуты сущности
        //            LdapAttributeSet attributeSet = nextEntry.GetAttributeSet();
        //            System.Collections.IEnumerator ienum = attributeSet.GetEnumerator();

        //            if (attributeSet.Count == 0)
        //            {
        //                continue;
        //            } 

        //            string UserLogin = "";
        //            string UserPassword = "";
        //            string UserFIO = "";
        //            // Проход по атрибутам сущности
        //            while (ienum.MoveNext())
        //            {
        //                LdapAttribute attribute = (LdapAttribute)ienum.Current;

        //                // uid, cn, userPassword
        //                string attributeName = attribute.Name;
        //                string attributeVal = attribute.StringValue;

        //                if (attributeName.Contains("uid"))
        //                {
        //                    UserLogin = attributeVal;
        //                }
        //                else if (attributeName.Contains("userPassword"))
        //                {
        //                    UserPassword = attributeVal;
        //                }
        //                else if (attributeName.Contains("cn"))
        //                {
        //                    UserFIO = attributeVal;
        //                }
        //            }
        //            Console.Write($"{UserFIO} {UserLogin} {UserPassword}");
        //        }
        //        conn.Disconnect();
        //    }
        //    catch (LdapException e)
        //    {
        //        Console.WriteLine("Error :" + e.LdapErrorMessage);
        //        return;
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine("Error :" + e.Message);
        //        return;
        //    }
        //}

        [HttpGet(Name = "TestDownload")]
        public async Task Download()
        {
            GetAllUsers();
        }


        //lk.ulstu, port:389
        //cn=attendance,ou=services,dc=ams,dc=ulstu,dc=ru
        //password=YaLeKm+8ER+7&m&4&DA

        //ou=accounts,dc=ams,dc=ulstu,dc=ru - строка для учеников
        //(&(objectClass=ulstuPerson)(accountStatus=active)(!(iduniv=SYSTEMACC)))
        //

        private void GetAllUsers()
        {
            //// Строка подключения
            //string ldapPath = "LDAP://lk.ulstu:389/cn=attendance,ou=services,dc=ams,dc=ulstu,dc=ru";
            //string username = "cn=attendance,ou=services,dc=ams,dc=ulstu,dc=ru";
            //string password = "YaLeKm+8ER+7&m&4&DA";

            //// Создание объекта DirectoryEntry для подключения к LDAP
            //DirectoryEntry ldapConnection = new DirectoryEntry(ldapPath, username, password);

            //// Создание объекта DirectorySearcher для выполнения поиска
            //DirectorySearcher searcher = new DirectorySearcher(ldapConnection);

            //// Установка фильтра поиска
            //searcher.Filter = "(&(objectClass=ulstuPerson)(accountStatus=active)(!(iduniv=SYSTEMACC)))";

            //try
            //{
            //    // Выполнение поиска
            //    SearchResultCollection results = searcher.FindAll();

            //    // Обработка результатов
            //    foreach (SearchResult result in results)
            //    {
            //        // Получение значений атрибутов из результатов
            //        foreach (string propName in result.Properties.PropertyNames)
            //        {
            //            foreach (var propValue in result.Properties[propName])
            //            {
            //                Console.WriteLine($"{propName}: {propValue}");
            //            }
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine($"Error: {ex.Message}");
            //}
            //finally
            //{
            //    // Закрытие соединения
            //    ldapConnection.Dispose();
            //}
        }

        /// <summary>
        /// Performs a search in the LDAP server. This method is generic in its return value to show the power
        /// of searches. A less generic search method could be implemented to only search for users, for instance.
        /// </summary>
        /// <param name="baseDn">The distinguished name of the base node at which to start the search</param>
        /// <param name="ldapFilter">An LDAP filter as defined by RFC4515</param>
        /// <returns>A flat list of dictionaries which in turn include attributes and the distinguished name (DN)</returns>
        //private List<Dictionary<string, string>> Search(string baseDn, string ldapFilter)
        //{
        //    var request = new SearchRequest(baseDn, ldapFilter, SearchScope.Subtree, null);
        //    var response = (SearchResponse)connection.SendRequest(request);

        //    var result = new List<Dictionary<string, string>>();

        //    foreach (SearchResultEntry entry in response.Entries)
        //    {
        //        var dic = new Dictionary<string, string>();
        //        dic["DN"] = entry.DistinguishedName;

        //        foreach (string attrName in entry.Attributes.AttributeNames)
        //        {
        //            //For simplicity, we ignore multi-value attributes
        //            dic[attrName] = string.Join(",", entry.Attributes[attrName].GetValues(typeof(string)));
        //        }

        //        result.Add(dic);
        //    }

        //    return result;
        //}

        ///// <summary>
        ///// Adds a user to the LDAP server database. This method is intentionally less generic than the search one to
        ///// make it easier to add meaningful information to the database.
        ///// </summary>
        ///// <param name="user">The user to add</param>
        //private void AddUser(UserModel user)
        //{
        //    var sha1 = new SHA1Managed();
        //    var digest = Convert.ToBase64String(sha1.ComputeHash(System.Text.Encoding.UTF8.GetBytes(user.UserPassword)));

        //    var request = new AddRequest(user.DN, new DirectoryAttribute[] {
        //        new DirectoryAttribute("uid", user.UID),
        //        new DirectoryAttribute("ou", user.OU),
        //        new DirectoryAttribute("userPassword", "{SHA}" + digest),
        //        new DirectoryAttribute("objectClass", new string[] { "top", "account", "simpleSecurityObject" })
        //    });

        //    connection.SendRequest(request);
        //}

        ///// <summary>
        ///// This method shows how to modify an attribute.
        ///// </summary>
        ///// <param name="oldUid">Old user UID</param>
        ///// <param name="newUid">New user UID</param>
        //private void ChangeUserUid(string oldUid, string newUid)
        //{
        //    var oldDn = string.Format("uid={0},ou=users,dc=example,dc=com", oldUid);
        //    var newDn = string.Format("uid={0},ou=users,dc=example,dc=com", newUid);

        //    DirectoryRequest request = new ModifyDNRequest(oldDn, "ou=users,dc=example,dc=com", "uid=" + newUid);
        //    connection.SendRequest(request);

        //    request = new ModifyRequest(newDn, DirectoryAttributeOperation.Replace, "uid", new string[] { newUid });
        //    connection.SendRequest(request);
        //}

        ///// <summary>
        ///// This method shows how to delete anything by its distinguised name (DN).
        ///// </summary>
        ///// <param name="dn">Distinguished name of the entry to delete</param>
        //private void Delete(string dn)
        //{
        //    var request = new DeleteRequest(dn);
        //    connection.SendRequest(request);
        //}

        ///// <summary>
        ///// Searches for a user and compares the password.
        ///// We assume all users are at base DN ou=users,dc=example,dc=com and that passwords are
        ///// hashed using SHA1 (no salt) in OpenLDAP format.
        ///// </summary>
        ///// <param name="username">Username</param>
        ///// <param name="password">Password</param>
        ///// <returns>true if the credentials are valid, false otherwise</returns>
        //private bool ValidateUser(string username, string password)
        //{
        //    var sha1 = new SHA1Managed();
        //    var digest = Convert.ToBase64String(sha1.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)));
        //    var request = new CompareRequest(string.Format("uid={0},ou=users,dc=example,dc=com", username),
        //        "userPassword", "{SHA}" + digest);
        //    var response = (CompareResponse)connection.SendRequest(request);
        //    return response.ResultCode == ResultCode.CompareTrue;
        //}

        ///// <summary>
        ///// Another way of validating a user is by performing a bind. In this case the server
        ///// queries its own database to validate the credentials. It is defined by the server
        ///// how a user is mapped to its directory.
        ///// </summary>
        ///// <param name="username">Username</param>
        ///// <param name="password">Password</param>
        ///// <returns>true if the credentials are valid, false otherwise</returns>
        //private bool ValidateUserByBind(string username, string password)
        //{
        //    bool result = true;
        //    var credentials = new NetworkCredential(username, password);
        //    var serverId = new LdapDirectoryIdentifier(connection.SessionOptions.HostName);

        //    var conn = new LdapConnection(serverId, credentials);
        //    try
        //    {
        //        conn.Bind();
        //    }
        //    catch (Exception)
        //    {
        //        result = false;
        //    }

        //    conn.Dispose();

        //    return result;
        //}
    }
}
