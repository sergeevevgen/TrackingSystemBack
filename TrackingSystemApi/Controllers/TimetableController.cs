using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrackingSystem.Api.Shared.Dto.Group;
using TrackingSystem.Api.Shared.Dto.Subject;
using TrackingSystem.Api.Shared.Dto.User;
using TrackingSystem.Api.Shared.Enums;
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

        [HttpGet("timetable/current")]
        [Authorize(Roles = "Pupil")]
        public async Task<IActionResult> TimetableCurrentWeek()
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

            var response = await _subjectManager.GetTimetableCurrentWeek(new GroupGetTimetableDto
            {
                GroupId = user.GroupId.Value
            });

            if (response.IsSuccess)
                return Ok(response.Data);
            else
                return BadRequest(response.ErrorMessage);
        }

        [HttpGet("timetableTeacher/current")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> TimetableCurrentWeekTeacher()
        {
            var user = await _userManager.GetCurrentUserDataAsync(_httpContextAccessor);

            if (!TryValidateModel(user))
            {
                return BadRequest(ModelState);
            }

            var response = await _subjectManager.GetTimetableCurrentWeekTeacher(new TeacherGetTimetableDto
            {
                TeacherId = user.Id
            });

            if (response.IsSuccess)
                return Ok(response.Data);
            else
                return BadRequest(response.ErrorMessage);
        }

        [HttpGet("downloadTimetable")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DownLoadTimeTable()
        {
            //await _ldapDownloadManager.SynchWithLdap();
            //var response = await _parserManager.ParseTimetable();

            //return response.IsSuccess ? Ok(response.Data) : BadRequest(response.ErrorMessage);
            return null;
        }
    }
}
