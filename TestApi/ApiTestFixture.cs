using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackingSystem.Api.Shared.Dto.User;

namespace TestApi
{
    public class ApiTestFixture : IDisposable
    {
        private readonly string _baseUrl = "http://localhost:7001/api/v1/";
        public string Token { get; private set; }
        public string Refresh { get; private set; }
        public string UserId { get; private set; }
        public string Login = "dozorova.a";
        public string Password = "db6554";
        public string Group { get; set; }
        public string Subject { get; set; } = "CF24C76A-B306-4943-B01A-08DC778C244B";

        public ApiTestFixture()
        {
            var client = new RestClient(_baseUrl);
            var request = new RestRequest($"{_baseUrl}user/login", Method.Post);
            request.AddJsonBody(new UserLoginDto { Login = Login, Password = Password });

            var response = client.Execute<UserLoginResponseDto>(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Token = response.Data.AccessToken;
                Refresh = response.Data.RefreshToken;
                UserId = response.Data.Id;
            }
            else
            {
                throw new Exception("Unable to authenticate.");
            }
        }

        public void Dispose()
        {
            
        }
    }
}
