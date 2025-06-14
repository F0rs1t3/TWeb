namespace TWeb.DTOs
{
    public class CarAvailabilityDto
    {
        public bool IsAvailable { get; set; }
        public string? Reason { get; set; }
        public List<DateRange> ConflictingDates { get; set; } = new();
        public List<DateTime> SuggestedAlternativeDates { get; set; } = new();
    }

    public class DateRange
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}