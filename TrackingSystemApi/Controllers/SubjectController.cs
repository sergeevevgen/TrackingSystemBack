using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrackingSystem.Api.Shared.Dto.Subject;
using TrackingSystem.Api.Shared.IManagers.LogicManagers;

namespace TrackingSystem.Api.Controllers
{
    [Route("/api/v1/[controller]/")]
    [ApiController]
    public class SubjectController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserManager _userManager;
        private readonly ISubjectManager _subjectManager;

        public SubjectController(
            IHttpContextAccessor httpContextAccessor,
            IUserManager userManager,
            ISubjectManager subjectManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _subjectManager = subjectManager;
        }

        [HttpPost("mark/{id:guid}")]
        [Authorize(Roles = "Pupil")]
        public async Task<IActionResult> MarkLesson([FromRoute] Guid id)
        {
            var user = await _userManager.GetCurrentUserDataAsync(_httpContextAccessor);

            if (!TryValidateModel(user))
            {
                return BadRequest(ModelState);
            }

            if (user.GroupId is null)
            {
                return BadRequest("У этого пользователя пустая группа!");
            }

            var response = await _subjectManager.MarkSubject(new SubjectUserMarkDto
            {
                SubjectId = id,
                PupilId = user.Id,
                GroupId = user.GroupId.Value
            });

            if (response.IsSuccess)
                return Ok(response.Data);
            else
                return BadRequest(response.ErrorMessage);
        }
    }
}
