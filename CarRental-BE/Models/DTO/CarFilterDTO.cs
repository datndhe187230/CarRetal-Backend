namespace CarRental_BE.Models.DTO
{
    public class CarFilterDTO
    {
        public string? SortBy { get; set; }
        public string? SortDirection { get; set; }
        public string? Brand { get; set; }
        public string? Search { get; set; }
    }

}
