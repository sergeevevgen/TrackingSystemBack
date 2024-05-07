using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrackingSystem.Api.BusinessLogic.Managers;
using TrackingSystem.Api.Shared.Dto.Identity;
using TrackingSystem.Api.Shared.Dto.User;
using TrackingSystem.Api.Shared.IManagers;
using TrackingSystem.Api.Shared.IManagers.LogicManagers;

namespace TrackingSystem.Api.Controllers
{
    [Route("/api/v1/[controller]/[action]")]
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

        [HttpPost]
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

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken(
            [FromBody] RefreshTokenCommand command,
            CancellationToken token)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _identityManager.RefreshToken(command, token);
            if (response.IsSuccess)
                return Ok(response.Data);
            else
                return BadRequest(response.ErrorMessage);
        }

        [HttpGet("current")]
        [Authorize]
        public async Task<IActionResult> GetUserByIdentity()
        {
            return Ok(await _userManager.GetCurrentUserDataAsync(_httpContextAccessor));
        }

        [HttpGet("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> GetUserById([FromRoute] Guid id)
        {
            var command = new UserFindDto { Id = id };

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _userManager.FindUserById(command, default);

            return response.IsSuccess ? Ok(response.Data) : BadRequest(response.ErrorMessage);
        }

        [HttpGet("mark/{id:guid}")]
        [Authorize]
        public async Task<IActionResult> MarkLesson([FromRoute] Guid id)
        {
            var message = "Урок отмечен";

            return Ok(message);
        }

        [HttpGet("timetable/today")]
        [Authorize]
        public async Task<IActionResult> TimetableToday()
        {
            var message = "sss";

            return Ok(message);
        }
    }
}
