namespace CarRental_BE.Models.VO.Transaction
{
    public class TransactionVO
    {
        public long Amount { get; set; }
        public string? Message { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? Status { get; set; }
        public string? Type { get; set; }
    }
}
