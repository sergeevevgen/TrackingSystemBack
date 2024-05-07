using AngleSharp.Html.Parser;
using System.Text;
using TrackingSystem.Api.BusinessLogic.DownloadLk;
using TrackingSystem.Api.Shared.IManagers.LogicManagers;

namespace TrackingSystem.Api.BusinessLogic.Managers
{
    public class ParserManager : IParserManager
    {
        private readonly ILogger _logger;
        private const int startFileIndex = 3;
        private readonly LkClient _lkClient;
        private readonly IUserManager _userManager;

        public ParserManager(
            ILogger logger,
            LkClient lkClient,
            IUserManager userManager)
        {
            _logger = logger;
            _lkClient = lkClient;
            _userManager = userManager;
        }

        public async Task ParseTimetable()
        {
            int? tmp = await _lkClient.GetTeacherCount();
            int endFileIndex = (int)(tmp != null ? tmp : 0);

            for(int i = startFileIndex; i < endFileIndex; i++)
            {
                try
                {
                    await ParseTimetableForTeacher(i);
                }
                catch(Exception ex)
                {
                    _logger.Error(ex, $"Ошибка парсинга файла с расписанием c Id {i}: {ex.Message}");
                    throw;
                }
            }
        }

        public async Task ParseTimetableForTeacher(int id)
        {
            // Парсим страницу
            var html = await new HtmlParser()
                .ParseDocumentAsync(File.ReadAllText($"{_lkClient.downloadFolder}{id}.html", 
                Encoding.GetEncoding("windows-1251")));

            // Парсинг блока
            var data = html.GetElementsByTagName("p")[0].Children[1].InnerHtml.Split("<br>");
            
            // Вытаскивание имени учителя
            var name = System.Text.RegularExpressions.Regex.Replace(data[0], @"^\s|\s$", "");

            // Вытаскивание номера нечетной недели
            var firstWeek = int.Parse(System.Text.RegularExpressions.Regex.Replace(data[1], @"\D", ""));

            var tabels = html.GetElementsByTagName("table");

            int tid = _userManager.Create(name);

            for (int week = 0; week < tabels.Length; week++)
            {
                var rows = tabels[week].GetElementsByTagName("tbody")[0].GetElementsByTagName("tr");
                for (int i = 2; i < rows.Length; i++)
                {
                    var columns = rows[i].GetElementsByTagName("td");
                    for (int j = 1; j < columns.Length; j++)
                    {
                        var item = columns[j].GetElementsByTagName("p")[0].Children[0].InnerHtml.Split("<br>");
                        if (item.Length >= 3 && item[0] != "_" && item[0] != "-" && item[0] != " ")
                        {
                            (int did, string type) = GetOrCreateDiscipline(item[1]);

                            foreach (var gr in item[0].Split(","))
                            {
                                int gid = GetOrCreateGroup(gr);

                                int pid = GetOrCreatePlace(item[2]);

                                CreateOrUpdatePair(new Subject()
                                {
                                    Groupid = gid,
                                    Disciplineid = did,
                                    Teacherid = tid,
                                    PlaceId = pid,
                                    Type = type,
                                    Dayofweak = i - 2,
                                    Pairnumber = j - 1, // Эту место пары - первой пары нет, есть ток вторая
                                    Week = (firstWeek + week) % 2 == 0, // Тут мне не надо делать четной, нечетной. Тупо неделю сохраняю
                                    Isdifference = 2
                                });

                            }
                        }
                    }
                }
            }
        }
    }
}
