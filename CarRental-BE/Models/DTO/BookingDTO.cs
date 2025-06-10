namespace CarRental_BE.Models.DTO
{
    public class BookingDTO
    {
        public string CarName { get; set; }
        public string CarImageUrl { get; set; } // optional
        public string BookingNumber { get; set; }
        public string PickUpLocation { get; set; }
        public string DropOffLocation { get; set; }
        public DateTime PickupDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public decimal BasePrice { get; set; }
        public decimal Deposit { get; set; }
        public string PaymentType { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
    }

}
