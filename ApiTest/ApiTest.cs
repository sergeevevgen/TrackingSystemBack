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

namespace ApiTest
{
    public class ApiTests
    {
        private readonly string _baseUrl = "http://localhost:5021/api/v1/";
        private string token;
        [Fact]
        public void TestLogin()
        {
            var client = new RestClient(_baseUrl);
            var request = new RestRequest("/user/login", Method.Post);
            request.AddJsonBody(new { username = "test", password = "password" });

            var response = client.Execute<UserLoginResponseDto>(request);

            token = response.Data.AccessToken;

            Assert.Equal(200, (int)response.StatusCode);
            Assert.NotNull(response.Content);

            var jsonResponse = JsonConvert.DeserializeObject<dynamic>(response.Content);
            Assert.NotNull(jsonResponse.token);
        }

        [Fact]
        public void TestRefreshToken()
        {
            var client = new RestClient(_baseUrl);
            var request = new RestRequest("/User/refreshToken", Method.Post);
            request.AddHeader("Authorization", $"Bearer {token}");

            var response = client.Execute<RefreshTokenResponseDTO>(request);

            Assert.Equal(200, (int)response.StatusCode);
            Assert.NotNull(response.Content);

            var jsonResponse = JsonConvert.DeserializeObject<dynamic>(response.Content);
            Assert.True(jsonResponse.schedule.Count > 0);
        }

        [Fact]
        public void TestCurrent()
        {
            var client = new RestClient(_baseUrl);
            var request = new RestRequest("/User/refreshToken", Method.Get);
            request.AddHeader("Authorization", "Bearer your-jwt-token");

            var response = client.Execute<UserResponseDto>(request);

            Assert.Equal(200, (int)response.StatusCode);
            Assert.NotNull(response.Content);

            var jsonResponse = JsonConvert.DeserializeObject<dynamic>(response.Content);
            Assert.True(jsonResponse.schedule.Count > 0);
        }

        [Fact]
        public void TestGetPupilTimetable()
        {
            var client = new RestClient(_baseUrl);
            var request = new RestRequest("/User/refreshToken", Method.Get);
            request.AddHeader("Authorization", "Bearer your-jwt-token");

            var response = client.Execute<UserGetTimetableResponseDto>(request);

            Assert.Equal(200, (int)response.StatusCode);
            Assert.NotNull(response.Content);

            var jsonResponse = JsonConvert.DeserializeObject<dynamic>(response.Content);
            Assert.True(jsonResponse.schedule.Count > 0);
        }

        [Fact]
        public void TestGetTeacherTimetable()
        {
            var client = new RestClient(_baseUrl);
            var request = new RestRequest("/User/refreshToken", Method.Get);
            request.AddHeader("Authorization", "Bearer your-jwt-token");

            var response = client.Execute<UserGetTimetableResponseDto>(request);

            Assert.Equal(200, (int)response.StatusCode);
            Assert.NotNull(response.Content);

            var jsonResponse = JsonConvert.DeserializeObject<dynamic>(response.Content);
            Assert.True(jsonResponse.schedule.Count > 0);
        }

        [Fact]
        public void TestMark()
        {
            var client = new RestClient(_baseUrl);
            var request = new RestRequest("/User/refreshToken", Method.Post);
            request.AddHeader("Authorization", "Bearer your-jwt-token");

            var response = client.Execute<string>(request);

            Assert.Equal(200, (int)response.StatusCode);
            Assert.NotNull(response.Content);

            var jsonResponse = JsonConvert.DeserializeObject<dynamic>(response.Content);
            Assert.True(jsonResponse.schedule.Count > 0);
        }
    }
}
