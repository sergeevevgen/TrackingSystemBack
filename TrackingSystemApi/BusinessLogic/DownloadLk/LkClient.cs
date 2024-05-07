using AngleSharp.Html.Parser;
using System.Text;

namespace TrackingSystem.Api.BusinessLogic.DownloadLk
{
    /// <summary>
    /// Http-клиент для загрузки .html страничек с расписанием с сайта УлГТУ и сохранением их в папку Download
    /// </summary>
    public class LkClient : HttpClient
    {
        // Todo: перенести в AppConfig
        private readonly string authURL = "https://lk.ulstu.ru/?q=auth/login";
        private readonly string teachersURL = "https://lk.ulstu.ru/timetable/shared/teachers/Praspisan.html";
        private readonly string timetableURL = "https://lk.ulstu.ru/timetable/shared/teachers/m";
        public readonly string downloadFolder = "Download\\";
        private const string fileName = "result.html";

        private readonly ILogger _logger;

        public LkClient(ILogger logger)
        {
            _logger = logger;
        }

        public async Task Authentication(string login, string password)
        {
            _logger.Info("Начало аутентификации");
            try
            {
                await PostAsync(
                    authURL,
                    new FormUrlEncodedContent(
                        new Dictionary<string, string>
                        {
                            { "login", login },
                            { "password", password }
                        }   
                    )
                );
                _logger.Info("Пользователь для загрузки расписания аутентифицирован успешно");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Ошибка авторизации в {authURL}");
                throw;
            }
        }

        public async Task GetRequestToFile(string url)
        {
            try
            {
                var test = await GetStringAsync(url);

                File.WriteAllText(fileName, test);
            }
            catch(Exception ex)
            {
                _logger.Error(ex, $"Ошибка загрузки файла из {url}");
                throw;
            }
        }

        public async Task<bool> DownloadFile(int id, string url)
        {
            try
            {
                var response = await GetByteArrayAsync($"{url}{id}.html");

                var html = Encoding.GetEncoding("windows-1251").GetString(response, 0, response.Length - 1).Replace("</br>", "<br>").Replace(".", "");

                if (File.Exists($"{downloadFolder}{id}.html"))
                {
                    var a = File.ReadAllText($"{downloadFolder}{id}.html", Encoding.GetEncoding("windows-1251"));
                    
                    if (a == html) {
                        return false; 
                    }
                }

                File.WriteAllText($"{downloadFolder}{id}.html", html, Encoding.GetEncoding("windows-1251"));
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Ошибка загрузки файла #{id}");
                throw;
            }
        }

        public async Task<int?> GetTeacherCount()
        {
            try
            {
                var stream = await GetStreamAsync(teachersURL);

                var document = new HtmlParser().ParseDocument(stream);

                int? count = document.All
                    .Where(x => x.LocalName == "table")
                    .FirstOrDefault()?.Children
                    .FirstOrDefault()?.Children.Length;

                if (!count.HasValue)
                {
                    return null;
                }
                else
                {
                    count--;
                }

                return count;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Ошибка при получении количества преподавателей");
                throw;
            }
        }

        public async Task<bool> TotalDownload(string path)
        {
            try
            {
                int? teachersCount = await GetTeacherCount();

                if (!teachersCount.HasValue)
                {
                    Console.WriteLine("При попытке узнать количество учителей возникла проблема");
                    return false;
                }

                Console.WriteLine($"Текущее колличество учителей: {teachersCount}");

                bool result = false;

                for (int i = 1; i < teachersCount; i++)
                {
                    bool res = await DownloadFile(i, timetableURL);

                    if (res)
                    { 
                        Console.WriteLine($"Обновление файла преподавателя с id={i}"); 
                    }

                    result |= res;
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Ошибка при получении количества преподавателей");
                throw;
            }
        }
    }
}
