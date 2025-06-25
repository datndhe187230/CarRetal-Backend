namespace CarRental_BE.Models.VO.Statistic
{
    public class TransactionTypeCountVO
    {
        public string Type { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
