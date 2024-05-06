namespace TrackingSystem.Api.Shared.Dto.Place
{
    public class PlaceDto
    {
        public Guid? Id { get; set; }

        public string? Name { get; set; }

        public ICollection<Guid>? Subjects { get; set; }
    }
}
