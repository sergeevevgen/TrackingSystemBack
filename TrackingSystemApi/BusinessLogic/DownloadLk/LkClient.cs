namespace TrackingSystem.Api.BusinessLogic.DownloadLk
{
    public class LkClient : HttpClient
    {
        public static string authURL = "https://lk.ulstu.ru/?q=auth/login";
        public static string teachersURL = "https://lk.ulstu.ru/timetable/shared/teachers/Praspisan.html";
        public static string timetableURL = "https://lk.ulstu.ru/timetable/shared/teachers/m";
        public static string downloadFolder = "Download\\";

        public static async Task Authentication(this HttpClient client, string login, string password)
        {
            await client.PostAsync(authURL, new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "login", login},
                {"password", password}
            }));
        }

        public static async Task GetRequestToFile(this HttpClient client, string url)
        {
            var test = await client.GetStringAsync(url);
            //var file = File.Create("resout.html");
            File.WriteAllText("resout.html", test);
        }

        public static async Task<bool> DownloadFile(this HttpClient client, int id, string url, string path)
        {
            var response = await client.GetByteArrayAsync($"{url}{id}.html");
            var html = Encoding.GetEncoding("windows-1251").GetString(response, 0, response.Length - 1).Replace("</br>", "<br>").Replace(".", "");
            if (File.Exists($"{downloadFolder}{id}.html"))
            {
                var a = File.ReadAllText($"{downloadFolder}{id}.html", Encoding.GetEncoding("windows-1251"));
                if (a == html) return false;
            }
            File.WriteAllText($"{downloadFolder}{id}.html", html, Encoding.GetEncoding("windows-1251"));
            return true;
        }

        public static async Task<int?> GetTeacherCount(this HttpClient client)
        {
            var stream = await client.GetStreamAsync(teachersURL);
            var document = new HtmlParser().ParseDocument(stream);
            int? count = document.All.Where(x => x.LocalName == "table").FirstOrDefault()?.Children.FirstOrDefault()?.Children.Count();
            if (!count.HasValue) return null;
            else count--;
            return count;
        }

        public static async Task<bool> TotalDownload(this HttpClient client, string path)
        {
            int? teachersCount = await client.GetTeacherCount();
            if (!teachersCount.HasValue)
            {
                Console.WriteLine("При попытке узнать колличество учителей возникла проблема.");
                return false;
            }
            Console.WriteLine($"Текущее колличество учителей {teachersCount} человек.");

            bool resout = false;

            for (int i = 1; i < teachersCount; i++)
            {
                bool res = await client.DownloadFile(i, timetableURL, path);
                if (res) Console.WriteLine($"Обновление файла преподавателя с id={i}");
                resout |= res;
            }
            return resout;
        }
    }
}
