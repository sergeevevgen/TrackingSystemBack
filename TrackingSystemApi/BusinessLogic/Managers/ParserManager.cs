using AngleSharp.Html.Parser;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.RegularExpressions;
using TrackingSystem.Api.AppLogic.Core;
using TrackingSystem.Api.BusinessLogic.DownloadLk;
using TrackingSystem.Api.Shared.Dto.Group;
using TrackingSystem.Api.Shared.Dto.Lesson;
using TrackingSystem.Api.Shared.Dto.Place;
using TrackingSystem.Api.Shared.Dto.Subject;
using TrackingSystem.Api.Shared.Dto.User;
using TrackingSystem.Api.Shared.Enums;
using TrackingSystem.Api.Shared.IManagers.LogicManagers;
using TrackingSystem.Api.Shared.SharedModels;

namespace TrackingSystem.Api.BusinessLogic.Managers
{
    public class ParserManager : IParserManager
    {
        private readonly ILogger _logger;
        private const int startFileIndex = 3;
        private readonly LkClient _lkClient;
        private readonly IUserManager _userManager;
        private readonly ILessonManager _lessonManager;
        private readonly IGroupManager _groupManager;
        private readonly IPlaceManager _placeManager;
        private readonly ISubjectManager _subjectManager;
        private readonly AppConfig _appConfig;

        public ParserManager(
            ILogger logger,
            LkClient lkClient,
            IUserManager userManager,
            ILessonManager lessonManager,
            IGroupManager groupManager,
            IPlaceManager placeManager,
            ISubjectManager subjectManager,
            IOptions<AppConfig> options)
        {
            _logger = logger;
            _lkClient = lkClient;
            _userManager = userManager;
            _lessonManager = lessonManager;
            _groupManager = groupManager;
            _placeManager = placeManager;
            _subjectManager = subjectManager;
            _appConfig = options.Value;
        }

        /// <summary>
        /// Основной метод для парсинга расписания
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseModel<string>> ParseTimetable()
        {
            await _lkClient.Authentication(_appConfig.LkLogin, _appConfig.LkPassword);

            await _lkClient.TotalDownload();

            int? tmp = await _lkClient.GetTeacherCount();
            int endFileIndex = (int)(tmp != null ? tmp : 0);

            // Флаг того, что надо isDifference текущей недели сделать = 0
            bool flag = true;

            for(int i = startFileIndex; i < endFileIndex; i++)
            {
                try
                {
                    flag = await ParseTimetableForTeacher(i, flag);
                }
                catch(Exception ex)
                {
                    var message = $"Ошибка парсинга файла с расписанием c Id {i}: {ex.Message}";
                    _logger.Error(ex, message);
                    return new ResponseModel<string> { ErrorMessage = message };
                }
            }

            _logger.Info($"Парсинг расписания прошёл успешно в {DateTime.Now}");

            return new ResponseModel<string> { Data = "Расписание обновлено" };
        }

        private async Task<bool> ParseTimetableForTeacher(int id, bool flag)
        {
            // Парсим страницу
            var html = await new HtmlParser()
                .ParseDocumentAsync(File.ReadAllText($"{_lkClient.downloadFolder}{id}.html", 
                Encoding.GetEncoding("windows-1251")));

            // Парсинг блока
            var data = html.GetElementsByTagName("p")[0].Children[1].InnerHtml.Split("<br>");
            
            // Вытаскивание имени учителя
            var name = Regex.Replace(data[0], @"^\s|\s$", "");

            // Вытаскивание номера нечетной недели
            var firstWeek = int.Parse(Regex.Replace(data[1], @"\D", ""));

            var tabels = html.GetElementsByTagName("table");

            // Обновляем расписание только по тем учителям, что есть в бд, иначе нет
            var teacher = await _userManager.Read(new UserDto { Name = name }, default);

            if (teacher == null)
            {
                return flag;
            }

            for (int week = 0; week < tabels.Length; week++)
            {
                // Делаем isDifference = 0 для каждой недели
                if (flag)
                {
                    await _subjectManager.ChangeIsDifferenceByWeek(new SubjectChangeIsDifferenceByWeekDto
                    {
                        Week = firstWeek + week
                    }, default);
                }

                var rows = tabels[week].GetElementsByTagName("tbody")[0].GetElementsByTagName("tr");
                for (int i = 2; i < rows.Length; i++)
                {
                    var columns = rows[i].GetElementsByTagName("td");
                    for (int j = 1; j < columns.Length; j++)
                    {
                        var item = columns[j].GetElementsByTagName("p")[0].Children[0].InnerHtml.Split("<br>");
                        if (item.Length >= 3 && item[0] != "_" && item[0] != "-" && item[0] != " ")
                        {
                            // Создаем или получаем тип занятия - да, потому что из лдапа мы можем вытащить только группы и пользователей
                            var lesson = await FormatLesson(item[1]);

                            foreach (var gr in item[0].Split(","))
                            {
                                // Получаем группу, если ее нет в БД, то выходим из парсинга
                                var group = await _groupManager.Read(new GroupDto { Name = gr }, default);

                                if (group == null)
                                {
                                    return flag;
                                } 

                                // Помещение создаем или получаем
                                var place = await _placeManager.CreateOrUpdate(new PlaceDto { Name = item[2] }, default);

                                // Надо протестить всё это
                                var result = await _subjectManager.CreateOrUpdate(new SubjectDto
                                {
                                    GroupId = group.Data.Id,
                                    LessonId = lesson.data.Id,
                                    PlaceId = place.Id,
                                    TeacherId = teacher.Data.Id,
                                    Day = i - 2,
                                    Pair = (EPairNumbers) (j - 1),
                                    Week = firstWeek + week,
                                    Type = lesson.type,
                                    IsDifference = EIsDifference.Actual
                                }, default);
                            }
                        }
                    }
                }
            }

            return !flag;
        }

        /// <summary>
        /// Вспомогательный метод для парсинга названия типа занятия
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private async Task<(LessonResponseDto data, string type)> FormatLesson(string name)
        {
            string type = "";
            string tmp = name.ToLower();

            if (tmp.StartsWith("пр"))
            {
                type = "пр";
                tmp = name[2..];
            }
            else if (tmp.StartsWith("лек"))
            {
                type = "лек";
                tmp = name[3..];
            }
            else if (tmp.StartsWith("лаб"))
            {
                type = "лаб";
                tmp = name[3..];
            }

            var lessonType = await _lessonManager
                .CreateOrUpdate(new LessonDto 
                { 
                    Name = tmp
                }, default);

            return (lessonType, type);
        }
    }
}
