using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrackingSystem.Api.BusinessLogic.Managers;
using TrackingSystem.Api.Shared.Dto.Identity;
using TrackingSystem.Api.Shared.Dto.Subject;
using TrackingSystem.Api.Shared.Dto.User;
using TrackingSystem.Api.Shared.Enums;
using TrackingSystem.Api.Shared.IManagers;
using TrackingSystem.Api.Shared.IManagers.LogicManagers;

namespace TrackingSystem.Api.Controllers
{
    [Route("/api/v1/[controller]/")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserManager _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IIdentityManager _identityManager;

        public UserController(
            IUserManager userManager,
            IHttpContextAccessor httpContextAccessor,
            IIdentityManager identityManager)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _identityManager = identityManager;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginAsync(
            [FromBody] UserLoginDto query,
            CancellationToken token)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _userManager.UserLoginAsync(query, token);

            if (response.IsSuccess)
                return Ok(response.Data);
            else
                return BadRequest(response.ErrorMessage);
        }

        [HttpPost("refreshToken")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken(
            [FromBody] RefreshTokenDto refreshDto,
            CancellationToken token)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = _identityManager.RefreshToken(refreshDto, token);

            if (response.IsSuccess)
                return Ok(response.Data);
            else
                return BadRequest(response.ErrorMessage);
        }

        [HttpGet("current")]
        [Authorize]
        public async Task<IActionResult> GetUserByIdentity()
        {
            var user = await _userManager.GetCurrentUserDataAsync(_httpContextAccessor);
            return Ok();
        }

        [HttpGet("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> GetUserById([FromRoute] Guid id)
        {
            var userDto = new UserFindDto { Id = id };

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _userManager.FindUserById(userDto, default);

            return response.IsSuccess ? Ok(response.Data) : BadRequest(response.ErrorMessage);
        }

        [HttpGet("changeInfo")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeInfo(
            [FromBody] InfoChangeDto dto)
        {
            var response = await _userManager.ChangeInfo(dto);

            if (response.IsSuccess)
            {
                return Ok("Успешно изменено");
            }

            return BadRequest(response.ErrorMessage);
        }
    }
}
