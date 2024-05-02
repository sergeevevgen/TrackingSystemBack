namespace TrackingSystem.Api.Shared.Dto.Group
{
    public class GroupDto
    {
        public Guid? Id { get; set; }

        public string Name { get; set; }

        public ICollection<Guid> Users { get; set; }

        public ICollection<Guid> Subjects { get; set; }
    }
}
