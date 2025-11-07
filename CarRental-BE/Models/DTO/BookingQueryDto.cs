namespace CarRental_BE.Models.DTO
{
    public class BookingQueryDto
    {
        public string? SearchTerm { get; set; }
        public string? SortOrder { get; set; } // "newest", "oldest"
        public List<string>? Statuses { get; set; }
    }
}
