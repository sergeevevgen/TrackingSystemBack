using TrackingSystem.Api.Shared.IManagers;

namespace TrackingSystem.Api.BusinessLogic.Managers
{
    public class ParserManager : IParserManager
    {
        private readonly IParserDbManager _dbManager;
        static readonly DateTime startFirstWeek = new DateTime(2023, 09, 4);

        Dictionary<string, Teacher> teachers;
        Dictionary<string, Discipline> disciplines;
        Dictionary<string, Group> groups;
        Dictionary<string, Place> places;
        List<Subject> subjects;
        Dictionary<string, Teacher> newTeachers = new();
        Dictionary<string, Discipline> newDisciplines = new();
        Dictionary<string, Group> newGroups = new();
        Dictionary<string, Place> newPlaces = new();
        int teacherIndex;
        int disciplineIndex;
        int groupIndex;
        int placeIndex;
        int subjectIndex;

        public ParserManager(IParserDbManager dbManager)
        {
            _dbManager = dbManager;
        }

        public async Task LoadDataFromServer()
        {
            await PrepareTimetable();
        }

        /// <summary>
        /// Удаляет все занятия с IsDifference == 0
        /// </summary>
        /// <returns></returns>
        private async Task PrepareTimetable()
        {
            await _dbManager.DeleteSubjectIsDifferenceZero();
        }

        private static void PullDataFromDB()
        {
            teachers = context.Teachers.ToDictionary(x => x.Name, x => x);
            disciplines = context.Disciplines.ToDictionary(x => x.Name, x => x);
            groups = context.Groups.ToDictionary(x => x.Name, x => x);
            places = context.Places.ToDictionary(x => x.Name, x => x);
            subjects = context.Subjects.ToList();

            newTeachers = new();
            newDisciplines = new();
            newGroups = new();
            newPlaces = new();

            teacherIndex = teachers.Values.Max(x => x.Id);
            groupIndex = groups.Values.Max(x => x.Id);
            placeIndex = places.Values.Max(x => x.Id);
            disciplineIndex = disciplines.Values.Max(x => x.Id);
            if (subjects.Count != 0) subjectIndex = subjects.Max(x => x.Id);
            else subjectIndex = 0;
        }
    }
}
