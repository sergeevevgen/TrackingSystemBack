using Microsoft.AspNetCore.Http;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TrackingSystem.Shared.Dto;
using TrackingSystem.Shared.Dto.User;
using TrackingSystem.Shared.IManagers;
using TrackingSystem.Shared.SharedModels;

namespace TrackingSystem.BusinessLogic.Managers
{
    public class UserManager : IUserManager
    {
        private readonly ILogger _logger;

        public UserManager(
            ILogger logger
            )
        {
            _logger = logger;
        }

        public Task<UserResponseDto> GetCurrentUserDataAsync(IHttpContextAccessor httpContextAccessor)
        {
            try
            {
                var userId = GetCurrentUserIdByContext(httpContextAccessor);

                //var userData = await _mediator.Send(new UserDataQuery
                //{
                //    Id = userId,
                //});

                return null;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Ошибка получения данных о пользователе");
                throw;
            }
        }

        public Guid GetCurrentUserIdByContext(IHttpContextAccessor httpContextAccessor)
        {
            try
            {
                var identity = httpContextAccessor.HttpContext?.User.Identity as ClaimsIdentity
                    ?? throw new Exception("Ошибка получения ClaimsIdentity");

                if (!identity.IsAuthenticated)
                    throw new AuthenticationException("Пользователь не вошёл в систему");

                var id = identity.FindFirst("Id")?.Value;
                if (string.IsNullOrEmpty(id))
                    throw new AuthenticationException("Невозможно определить Id пользователя в контексте сервера");

                return Guid.Parse(id);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Ошибка определения Id пользователя");
                throw;
            }
        }

        public async Task<ResponseModel<UserLoginResponseDto>> UserLoginAsync(
            UserLoginDto query,
            CancellationToken cancellationToken)
        {
            try
            {
                var user = await _mediator.Send(
                    new UserDataQuery
                    {
                        Login = query.Login,
                        Password = query.Password,
                    },
                    cancellationToken);

                if (user == null)
                    return new ResponseModel<UserLoginResponseDto> { ErrorMessage = "Неправильный логин / пароль" };

                if (!user.IsConfirmed)
                    return new ResponseModel<UserLoginResponseDto> { ErrorMessage = "Учетная запись не подтверждена" };

                var identity = await _mediator.Send(
                    new CreateIdentityCommand
                    {
                        Login = user.Login,
                        UserId = user.Id,
                        // Role = TODO
                    },
                    cancellationToken);

                var accessToken = _jwtAuthManager.GenerateToken(
                    identity.Claims,
                    JwtTokenCommandType.Access);

                var refreshToken = _jwtAuthManager.GenerateToken(
                    identity.Claims,
                    JwtTokenCommandType.Refresh);

                var response = new ResponseModel<UserLoginResponseDto>
                {
                    Data = new UserLoginResponseDto
                    {
                        AccessToken = accessToken,
                        RefreshToken = refreshToken,
                        Name = identity.Name,
                        Id = identity.Claims.Last().Value,
                        //TODO role
                    }
                };

                return response;
            }
            catch(Exception ex)
            {
                var errorMessage = "Неправильный логин / пароль";
                _logger.Error(ex, errorMessage);
                return new ResponseModel<UserLoginResponseDto> { ErrorMessage = errorMessage };
            }
        }
    }
}
