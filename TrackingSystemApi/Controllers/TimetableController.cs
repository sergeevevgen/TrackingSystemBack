using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrackingSystem.Api.Shared.Dto.Subject;
using TrackingSystem.Api.Shared.Dto.User;
using TrackingSystem.Api.Shared.IManagers.LogicManagers;

namespace TrackingSystem.Api.Controllers
{
    [Route("/api/v1/[controller]/")]
    [ApiController]
    public class TimetableController : ControllerBase
    {
        private readonly IParserManager _parserManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserManager _userManager;
        private readonly ISubjectManager _subjectManager;
        private readonly ILdapDownloadManager _ldapDownloadManager;

        public TimetableController(
            IUserManager userManager,
            IParserManager parserManager,
            IHttpContextAccessor httpContextAccessor,
            ISubjectManager subjectManager,
            ILdapDownloadManager ldapDownloadManager)
        {
            _parserManager = parserManager;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _subjectManager = subjectManager;
            _ldapDownloadManager = ldapDownloadManager;
        }

        [HttpGet("mark/{id:guid}")]
        [Authorize(Roles = "Pupil")]
        public async Task<IActionResult> MarkLesson([FromRoute] Guid id)
        {
            var user = await _userManager.GetCurrentUserDataAsync(_httpContextAccessor);

            if (!TryValidateModel(user))
            {
                return BadRequest(ModelState);
            }

            var response = await _subjectManager.MarkSubject(new SubjectUserMarkDto
            {
                SubjectId = id,
                PupilId = user.Id,
            });

            if (response.IsSuccess)
                return Ok(response.Data);
            else
                return BadRequest(response.ErrorMessage);
        }

        [HttpGet("timetable/today")]
        [Authorize(Roles = "Pupil")]
        public async Task<IActionResult> TimetableToday()
        {
            var user = await _userManager.GetCurrentUserDataAsync(_httpContextAccessor);

            if (!TryValidateModel(user))
            {
                return BadRequest(ModelState);
            }

            if (user.GroupId is null)
            {
                return BadRequest();
            }

            var response = await _subjectManager.GetTimetableToday(new GroupGetTimetableDto
            {
                GroupId = user.GroupId.Value
            });

            if (response.IsSuccess)
                return Ok(response.Data);
            else
                return BadRequest(response.ErrorMessage);
        }

        [HttpGet("downloadTimetable")]
        [AllowAnonymous]
        public async Task<IActionResult> DownLoadTimeTable()
        {
            await _ldapDownloadManager.SynchWithLdap();
            var response = await _parserManager.ParseTimetable();

            return response.IsSuccess ? Ok(response.Data) : BadRequest(response.ErrorMessage);
        }
    }
}
