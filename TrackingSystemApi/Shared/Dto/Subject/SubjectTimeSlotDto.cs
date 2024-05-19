namespace TrackingSystem.Api.Shared.Dto.Subject
{
    /// <summary>
    /// Модель для времени определенных пар 
    /// </summary>
    public class SubjectTimeSlotDto
    {
        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }
    }
}
