namespace TrackingSystem.Api.Shared.Dto.Lesson
{
    public class LessonHelpDto
    {
        public Guid GroupId { get; set; }

        public string? GroupName { get; set; }

        public Guid StudentId { get; set; }

        public string? StudentName { get; set; }

        public string? MarkTime { get; set; }
    }
}
