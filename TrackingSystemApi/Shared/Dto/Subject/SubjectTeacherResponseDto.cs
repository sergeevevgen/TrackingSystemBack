namespace TrackingSystem.Api.Shared.Dto.Subject
{
    public class SubjectTeacherResponseDto
    {
        public Guid SubjectId { get; set; }

        public string? SubjectName { get; set; }

        public string? GroupName { get; set; }

        public string? PlaceName { get; set; }

        public string? From { get; set; }
        
        public string? To { get; set; }

        public string? Date { get; set; }
    }
}
