using TrackingSystem.Api.Shared.Enums;

namespace TrackingSystem.Api.Shared.Dto.Subject
{
    public class TimetableResponseDto
    {
        public string GroupName { get; set; } = null!;

        public string TeacherName { get; set; } = null!;

        public string LessonName { get; set; } = null!;

        public PairNumber Number { get; set; }

        public string PlaceName { get; set; } = null!;

        public int Day { get; set; }

        public string Type { get; set; } = null!;
    }
}
