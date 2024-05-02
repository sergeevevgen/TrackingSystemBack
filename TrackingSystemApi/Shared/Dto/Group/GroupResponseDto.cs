namespace TrackingSystem.Api.Shared.Dto.Group
{
    public class GroupResponseDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public ICollection<Guid> Users { get; set; }

        public ICollection<Guid> Subjects { get; set; }
    }
}
