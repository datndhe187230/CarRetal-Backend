namespace CarRental_BE.Models.VO.Statistic
{
    public class TopPayingCustomerVO
    {
        public Guid AccountId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public decimal TotalPayments { get; set; }
        public int TotalBookings { get; set; }
        public string MemberSince { get; set; } = string.Empty;
        public string LastBooking { get; set; } = string.Empty;
        public string PreferredVehicle { get; set; } = string.Empty;
    }
}
