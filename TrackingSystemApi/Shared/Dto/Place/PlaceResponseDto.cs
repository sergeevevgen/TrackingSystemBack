namespace TrackingSystem.Api.Shared.Dto.Place
{
    public class PlaceResponseDto
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public ICollection<Guid>? Subjects { get; set; }
    }
}
