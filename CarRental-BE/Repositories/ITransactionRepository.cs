using CarRental_BE.Models.VO.Statistic;

namespace CarRental_BE.Repositories
{
    public interface ITransactionRepository
    {
        Task<decimal> GetTotalTransactionAmountAsync();
        Task<IEnumerable<TopPayingCustomerVO>> GetTopPayingCustomersAsync(int count = 5);
        Task<IEnumerable<TransactionTypeCountVO>> GetTransactionTypeCountsAsync();
        Task<IEnumerable<DailyTransactionVO>> GetDailyTransactionsAsync(DateTime startDate, DateTime endDate);
    }
}
