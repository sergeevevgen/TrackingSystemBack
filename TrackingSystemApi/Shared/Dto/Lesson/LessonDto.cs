namespace TrackingSystem.Api.Shared.Dto.Lesson
{
    public class LessonDto
    {
        public Guid? Id { get; set; }

        public string? Name { get; set; }

        public ICollection<Guid>? Subjects { get; set; }
    }
}
