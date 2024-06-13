using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrackingSystem.Api.Shared.Dto.Lesson;
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
        private readonly ILessonManager _lessonManager;

        public SubjectController(
            IHttpContextAccessor httpContextAccessor,
            IUserManager userManager,
            ISubjectManager subjectManager,
            ILessonManager lessonManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _subjectManager = subjectManager;
            _lessonManager = lessonManager;
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

        [HttpGet("current")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> GetCurrentLesson()
        {
            var user = await _userManager.GetCurrentUserDataAsync(_httpContextAccessor);

            if (!TryValidateModel(user))
            {
                return BadRequest(ModelState);
            }

            var response = await _subjectManager.GetCurrentSubjectByTeacher(new SubjectTeacherDto
            {
                TeacherId = user.Id,
            });

            if (response.IsSuccess)
                return Ok(response.Data);
            else
                return BadRequest(response.ErrorMessage);
        }

        [HttpGet("lessons")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> GetMyLessons()
        {
            var user = await _userManager.GetCurrentUserDataAsync(_httpContextAccessor);

            if (!TryValidateModel(user))
            {
                return BadRequest(ModelState);
            }

            // чтобы вывести список всех занятий препода, мне надо его гуид и впринципе всё
            var response = await _lessonManager.GetTeacherLessons(user.Id);

            if (response.IsSuccess)
                return Ok(response.Data);
            else
                return BadRequest(response.ErrorMessage);
        }

        [HttpGet("statistic/{id:guid}")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> GetLessonStatistic([FromRoute] Guid id)
        {
            var user = await _userManager.GetCurrentUserDataAsync(_httpContextAccessor);

            if (!TryValidateModel(user))
            {
                return BadRequest(ModelState);
            }

            // чтобы вывести список всех занятий препода, мне надо его гуид и впринципе всё
            var response = await _lessonManager.GetTeacherLessonStatistic(
                new TeacherLessonStatisticDto
                { 
                    LessonId = id, TeacherId = user.Id 
                }
            );

            if (response.IsSuccess)
                return Ok(response.Data);
            else
                return BadRequest(response.ErrorMessage);
        }
    }
}
