using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrackingSystem.Api.BusinessLogic.Managers;
using TrackingSystem.Api.Shared.Dto.Identity;
using TrackingSystem.Api.Shared.Dto.User;
using TrackingSystem.Api.Shared.IManagers;

namespace TrackingSystem.Api.Controllers
{
    //[Route("/api/v1/[controller]/[action]")]
    [Route("/[controller]/[action]")]
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

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            return Ok("stirng lsa;dasld;a");
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> LoginAsync(
            [FromBody] UserLoginQuery query,
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

        [HttpGet("/api/v1/users/current")]
        [Authorize]
        public async Task<IActionResult> GetUserByIdentity()
        {
            return Ok(await _userManager.GetCurrentUserDataAsync(_httpContextAccessor));
        }

        [HttpGet("/api/v1/users/{id:guid}")]
        [Authorize]
        public async Task<IActionResult> GetUserById([FromRoute] Guid id)
        {
            var command = new UserByIdQuery { UserId = id };

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _userManager.FindUserById(command, default);

            return response.IsSuccess ? Ok(response.Data) : BadRequest(response.ErrorMessage);
        }
    }
}
