namespace TrackingSystem.Api.Shared.Dto.Subject
{
    public class SubjectUserMarkDto
    {
        public Guid SubjectId { get; set; }

        public Guid PupilId { get; set; }

        public bool Mark { get; set; }

        public DateTime MarkTime { get; set; }
    }
}
