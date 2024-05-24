using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;
using TrackingSystem.Api.Shared.Dto.Identity;
using TrackingSystem.Api.Shared.Dto.User;
using Xunit;

namespace TestApi
{
    [Collection("ApiTest")]
    public class ApiTest
    {
        private readonly ApiTestFixture _fixture;

        public ApiTest(ApiTestFixture fixture)
        {
            _fixture = fixture;
        }

        private readonly string _baseUrl = "http://localhost:7001/api/v1/";

        [Fact]
        public void TestLogin()
        {
            Assert.NotNull(_fixture.Token);
        }

        [Fact]
        public void TestRefreshToken()
        {
            var client = new RestClient(_baseUrl);
            var request = new RestRequest($"{_baseUrl}user/refreshToken", Method.Post);
            request.AddJsonBody(new RefreshTokenDto { TokenHash = _fixture.Refresh });

            var response = client.Execute<RefreshTokenResponseDTO>(request);

            Assert.Equal(200, (int)response.StatusCode);
            Assert.NotNull(response.Content);

            var jsonResponse = JsonConvert.DeserializeObject<RefreshTokenResponseDTO>(response.Content);
            Assert.NotNull(jsonResponse.AccessToken);
        }

        [Fact]
        public void TestCurrent()
        {
            var client = new RestClient(_baseUrl);
            var request = new RestRequest($"{_baseUrl}user/current", Method.Get);
            request.AddHeader("Authorization", $"Bearer {_fixture.Token}");

            var response = client.Execute<UserResponseDto>(request);

            Assert.Equal(200, (int)response.StatusCode);
            Assert.NotNull(response.Content);

            var jsonResponse = JsonConvert.DeserializeObject<UserResponseDto>(response.Content);
            Assert.Equal(jsonResponse.Login, _fixture.Login);
        }

        [Fact]
        public void TestGetPupilTimetable()
        {
            var client = new RestClient(_baseUrl);
            var request = new RestRequest($"{_baseUrl}timetable/timetable/current", Method.Get);
            request.AddHeader("Authorization", $"Bearer {_fixture.Token}");

            var response = client.Execute<UserGetTimetableResponseDto>(request);

            Assert.Equal(200, (int)response.StatusCode);
            Assert.NotNull(response.Content);

            var jsonResponse = JsonConvert.DeserializeObject<UserGetTimetableResponseDto>(response.Content);
            Assert.NotNull(jsonResponse);
        }

        [Fact]
        public void TestGetTeacherTimetable()
        {
            var client = new RestClient(_baseUrl);
            var request = new RestRequest($"{_baseUrl}timetable/timetableTeacher/current", Method.Get);
            request.AddHeader("Authorization", $"Bearer {_fixture.Token}");

            var response = client.Execute<UserGetTimetableResponseDto>(request);

            Assert.Equal(System.Net.HttpStatusCode.Forbidden, response.StatusCode);
            Assert.NotNull(response.Content);

            var jsonResponse = JsonConvert.DeserializeObject<UserGetTimetableResponseDto>(response.Content);
            //Assert.Equal(15, jsonResponse.Week);
            Assert.Null(jsonResponse);
        }

        [Fact]
        public void TestMark()
        {
            var client = new RestClient(_baseUrl);
            var request = new RestRequest($"{_baseUrl}subject/mark/{_fixture.Subject}", Method.Post);
            request.AddHeader("Authorization", $"Bearer {_fixture.Token}");
            
            var response = client.Execute<string>(request);

            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
            Assert.NotNull(response.Content);

            var jsonResponse = JsonConvert.DeserializeObject<string>(response.Content);
            Assert.Equal(jsonResponse, $"Не тот день");
        }
    }
}