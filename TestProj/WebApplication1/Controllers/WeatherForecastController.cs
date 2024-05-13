using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.DirectoryServices;
using System.Runtime.CompilerServices;

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

        [HttpGet(Name = "Test2")]
        public async Task<string> Test2()
        {
            return "Not inmplemented";
        }

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
            // Строка подключения
            string ldapPath = "LDAP://lk.ulstu:389/cn=attendance,ou=services,dc=ams,dc=ulstu,dc=ru";
            string username = "cn=attendance,ou=services,dc=ams,dc=ulstu,dc=ru";
            string password = "YaLeKm+8ER+7&m&4&DA";

            // Создание объекта DirectoryEntry для подключения к LDAP
            DirectoryEntry ldapConnection = new DirectoryEntry(ldapPath, username, password);

            // Создание объекта DirectorySearcher для выполнения поиска
            DirectorySearcher searcher = new DirectorySearcher(ldapConnection);

            // Установка фильтра поиска
            searcher.Filter = "(&(objectClass=ulstuPerson)(accountStatus=active)(!(iduniv=SYSTEMACC)))";

            try
            {
                // Выполнение поиска
                SearchResultCollection results = searcher.FindAll();

                // Обработка результатов
                foreach (SearchResult result in results)
                {
                    // Получение значений атрибутов из результатов
                    foreach (string propName in result.Properties.PropertyNames)
                    {
                        foreach (var propValue in result.Properties[propName])
                        {
                            Console.WriteLine($"{propName}: {propValue}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                // Закрытие соединения
                ldapConnection.Dispose();
            }
        }
    }
}
