using TrackingSystem.Api.Shared.Enums;

namespace TrackingSystem.Api.Shared.Dto.Subject
{
    public class SubjectDto
    {
        public Guid? Id { get; set; }

        public int Week { get; set; }

        public int Day { get; set; }

        public string? Type { get; set; }

        public PairNumber Pair { get; set; }

        public Difference IsDifference { get; set; }

        public Guid GroupId { get; set; }

        public Guid PlaceId { get; set; }

        public Guid LessonId { get; set; }

        public Guid TeacherId { get; set; }

        public Dictionary<Guid, bool>? Users { get; set; }
    }
}
