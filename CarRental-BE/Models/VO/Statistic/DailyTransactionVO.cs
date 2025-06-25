namespace CarRental_BE.Models.VO.Statistic
{
    public class DailyTransactionVO
    {
        public DateTime Date { get; set; }
        public decimal TotalAmount { get; set; }
        public int TransactionCount { get; set; }
    }
}
