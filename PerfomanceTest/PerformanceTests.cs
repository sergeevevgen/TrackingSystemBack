using BenchmarkDotNet.Attributes;
using RestSharp;

namespace PerfomanceTest
{
    public class PerformanceTests
    {
        private readonly string _baseUrl = "http://localhost:7001/api/v1/";
        private string token;
        private string login = "dozorova.a";
        private string password = "db6554";
        private string subject = "CF24C76A-B306-4943-B01A-08DC778C244B";

        [GlobalSetup]
        public async Task Setup()
        {
            var client = new RestClient(_baseUrl);
            var request = new RestRequest("user/login", Method.Post);
            request.AddJsonBody(new UserLoginDto { Login = login, Password = password });

            var response = await client.ExecuteAsync<UserLoginResponseDto>(request);

            token = response.Data.AccessToken;
        }

        [Benchmark]
        public async Task TestCurrent()
        {
            var client = new RestClient(_baseUrl);
            var request = new RestRequest("user/current", Method.Get);
            request.AddHeader("Authorization", $"Bearer {token}");

            var response = await client.ExecuteAsync<UserResponseDto>(request);
        }

        [Benchmark]
        public async Task TestGetPupilTimetable()
        {
            var client = new RestClient(_baseUrl);
            var request = new RestRequest("timetable/timetable/current", Method.Get);
            request.AddHeader("Authorization", $"Bearer {token}");

            var response = await client.ExecuteAsync<UserGetTimetableResponseDto>(request);
        }

        [Benchmark]
        public async Task TestGetTeacherTimetable()
        {
            var client = new RestClient(_baseUrl);
            var request = new RestRequest("timetable/timetableTeacher/current", Method.Get);
            request.AddHeader("Authorization", $"Bearer {token}");

            var response = await client.ExecuteAsync<UserGetTimetableResponseDto>(request);
        }

        [Benchmark]
        public async Task TestMark()
        {
            var client = new RestClient(_baseUrl);
            var request = new RestRequest($"subject/mark/{subject}", Method.Post);
            request.AddHeader("Authorization", $"Bearer {token}");

            var response = await client.ExecuteAsync<string>(request);
        }
    }

    public class UserLoginDto
    {
        public string Login { get; init; } = null!;

        public string Password { get; init; } = null!;
    }

    public class UserLoginResponseDto
    {
        public string Id { get; init; } = null!;

        public string? Name { get; init; }

        public string AccessToken { get; init; } = null!;

        public string RefreshToken { get; init; } = null!;

        public string Role { get; init; } = null!;
    }

    public class UserResponseDto
    {
        public Guid Id { get; set; }

        public string Login { get; set; } = null!;

        public ERoles Role { get; set; }

        public string Name { get; set; } = null!;

        public string? Group { get; set; }

        public Guid? GroupId { get; set; }

        public EStatus? Status { get; set; }
    }

    public class UserGetTimetableResponseDto
    {
        public int Week { get; set; }

        public List<TimetableResponseDto> Timetable { get; set; } = null!;
    }

    public class TimetableResponseDto
    {
        public string GroupName { get; set; } = null!;

        public string TeacherName { get; set; } = null!;

        public string LessonName { get; set; } = null!;

        public EPairNumbers Number { get; set; }

        public string PlaceName { get; set; } = null!;

        public int Day { get; set; }

        public string Type { get; set; } = null!;
    }

    public enum EPairNumbers
    {
        First = 0,
        Second = 1,
        Third = 2,
        Fourth = 3,
        Fifth = 4,
        Sixth = 5,
        Seventh = 6,
        Eighth = 7,
    }

    public enum ERoles
    {
        Pupil = 0,
        Teacher = 1,
        Admin = 99
    }

    public enum EStatus
    {
        Is_In_Academ = 0,
        Is_Studying = 1,
        Is_Dropped = 2,
    }
}
