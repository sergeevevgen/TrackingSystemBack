namespace TrackingSystem.Api.Shared.Dto.User
{
    public class UserGetTimetableResponseDto
    {
        public int Week { get; set; }

        public List<TimetableResponseDto> Timetable { get; set; } = null!;
    }
}
