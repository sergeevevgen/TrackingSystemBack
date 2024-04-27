using AngleSharp.Html.Parser;
using System.Text;
using TrackingSystem.Api.DataLayer.Models;
using TrackingSystem.Api.Shared.IManagers;
namespace TrackingSystem.Api.BusinessLogic.Managers
{
    public class ParserManager : IParserManager
    {
        private readonly IParserDbManager _dbManager;

        public ParserManager(IParserDbManager dbManager)
        {
            _dbManager = dbManager;
        }

        public Task LoadDataFromServer()
        {
            throw new NotImplementedException();
        }
        //static readonly DateTime startFirstWeek = new DateTime(2023, 09, 4);

        //Dictionary<string, Teacher> teachers;
        //Dictionary<string, Discipline> disciplines;
        //Dictionary<string, Group> groups;
        //Dictionary<string, Place> places;
        //List<Subject> subjects;
        //Dictionary<string, Teacher> newTeachers = new();
        //Dictionary<string, Discipline> newDisciplines = new();
        //Dictionary<string, Group> newGroups = new();
        //Dictionary<string, Place> newPlaces = new();
        //int teacherIndex;
        //int disciplineIndex;
        //int groupIndex;
        //int placeIndex;
        //int subjectIndex;

        //public ParserManager(IParserDbManager dbManager)
        //{
        //    _dbManager = dbManager;
        //}

        //public async Task LoadDataFromServer()
        //{
        //    await PrepareTimetable();
        //}

        ///// <summary>
        ///// Удаляет все занятия с IsDifference == 0
        ///// </summary>
        ///// <returns></returns>
        //private async Task PrepareTimetable()
        //{
        //    await _dbManager.DeleteSubjectIsDifferenceZero();
        //}

        //private static void PullDataFromDB()
        //{
        //    teachers = context.Teachers.ToDictionary(x => x.Name, x => x);
        //    disciplines = context.Disciplines.ToDictionary(x => x.Name, x => x);
        //    groups = context.Groups.ToDictionary(x => x.Name, x => x);
        //    places = context.Places.ToDictionary(x => x.Name, x => x);
        //    subjects = context.Subjects.ToList();

        //    newTeachers = new();
        //    newDisciplines = new();
        //    newGroups = new();
        //    newPlaces = new();

        //    teacherIndex = teachers.Values.Max(x => x.Id);
        //    groupIndex = groups.Values.Max(x => x.Id);
        //    placeIndex = places.Values.Max(x => x.Id);
        //    disciplineIndex = disciplines.Values.Max(x => x.Id);
        //    if (subjects.Count != 0) subjectIndex = subjects.Max(x => x.Id);
        //    else subjectIndex = 0;
        //}

        /// <summary>
        /// Загрузка расписания учителя
        /// </summary>
        /// <param name="context"></param>
        /// <param name="id"></param>
        public async Task LoadTeacher(int id)
        {
            var html = new HtmlParser().ParseDocument(File.ReadAllText($@"Download\{id}.html", Encoding.GetEncoding("windows-1251")));

            var data = html.GetElementsByTagName("p")[0].Children[1].InnerHtml.Split("<br>");
            var name = System.Text.RegularExpressions.Regex.Replace(data[0], @"^\s|\s$", "");
            var firstWeek = int.Parse(System.Text.RegularExpressions.Regex.Replace(data[1], @"\D", ""));

            var tabels = html.GetElementsByTagName("table");

            int tid = _dbManager.GetOrCreateTeacher(name);

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
                                    Pairnumber = j - 1,
                                    Week = (firstWeek + week) % 2 == 0,
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
