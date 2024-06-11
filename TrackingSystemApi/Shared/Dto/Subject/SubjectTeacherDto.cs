using TrackingSystem.Api.Shared.Enums;

namespace TrackingSystem.Api.Shared.Dto.Subject
{
    public class SubjectTeacherDto
    {
        public Guid TeacherId { get; set; }
        public int Day {  get; set; }
        public PairNumber Pair { get; set; }
    }
}
