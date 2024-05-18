using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrackingSystem.Api.Shared.IManagers.LogicManagers;

namespace TrackingSystem.Api.Controllers
{
    [Route("/api/v1/[controller]/")]
    [ApiController]
    public class TimetableController : ControllerBase
    {
        private readonly IParserManager _parserManager;

        public TimetableController(IParserManager parserManager)
        {
            _parserManager = parserManager;
        }

        [HttpGet("mark/{id:guid}")]
        [Authorize]
        public async Task<IActionResult> MarkLesson([FromRoute] Guid id)
        {
            var message = "Урок отмечен";

            return Ok(message);
        }

        [HttpGet("timetable/today")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> TimetableToday()
        {
            var message = "sss";

            return Ok(message);
        }

        [HttpGet("test")]
        [Authorize(Roles = "Pupil")]
        public async Task<IActionResult> Test()
        {
            var message = "good access";

            return Ok(message);
        }

        [HttpGet("downloadTimetable")]
        [Authorize]
        public async Task<IActionResult> DownLoadTimeTable()
        {
            var response = await _parserManager.ParseTimetable();

            return response.IsSuccess ? Ok(response.Data) : BadRequest(response.ErrorMessage);
        }
    }
}
