using CarRental_BE.Data;
using CarRental_BE.Models.VO.Statistic;
using Microsoft.EntityFrameworkCore;

namespace CarRental_BE.Repositories.Impl
{
    public class TransactionRepositoryImpl : ITransactionRepository
    {
        private readonly CarRentalContext _context;

        public TransactionRepositoryImpl(CarRentalContext context)
        {
            _context = context;
        }

        public async Task<decimal> GetTotalTransactionAmountAsync()
        {
            return await _context.Transactions
                .Where(t => t.Status == "Completed")
                .SumAsync(t => (decimal)t.Amount);
        }

        public async Task<IEnumerable<TopPayingCustomerVO>> GetTopPayingCustomersAsync(int count = 5)
        {
            return await _context.Transactions
                .Include(t => t.Wallet)
                .Include(t => t.BookingNumberNavigation)
                .Include(t => t.Wallet.IdNavigation)
                .Include(t => t.Wallet.IdNavigation.UserProfile)
                .Where(t => t.Status == "Completed")
                .GroupBy(t => new { t.Wallet.IdNavigation})
                .Select(g => new TopPayingCustomerVO
                {
                    AccountId = g.Key.IdNavigation.Id,
                    Email = g.Key.IdNavigation.Email,
                    CustomerName = g.Key.IdNavigation.UserProfile.FullName ?? "Unknown",
                    TotalPayments = g.Sum(t => (decimal)t.Amount),
                    Phone = g.Key.IdNavigation.UserProfile.PhoneNumber ?? "Unknown",
                    TotalBookings = g.Select(t => t.BookingNumber).Distinct().Count(),
                    MemberSince = g.Key.IdNavigation.CreatedAt.Year.ToString() ?? "Unknown",
                    LastBooking = g.Max(t => t.CreatedAt).HasValue ?
                        (DateTime.Now - g.Max(t => t.CreatedAt).Value).Days + " days ago" : "Unknown",
                    PreferredVehicle = g.FirstOrDefault().CarName ?? "Unknown"
                })
                .OrderByDescending(x => x.TotalPayments)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<TransactionTypeCountVO>> GetTransactionTypeCountsAsync()
        {
            return await _context.Transactions
                .GroupBy(t => t.Type)
                .Select(g => new TransactionTypeCountVO
                {
                    Type = g.Key ?? "Unknown",
                    Count = g.Count(),
                    TotalAmount = g.Sum(t => (decimal)t.Amount)
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<DailyTransactionVO>> GetDailyTransactionsAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Transactions
                .Where(t => t.CreatedAt >= startDate && t.CreatedAt <= endDate)
                .GroupBy(t => t.CreatedAt.Value.Date)
                .Select(g => new DailyTransactionVO
                {
                    Date = g.Key,
                    TotalAmount = g.Sum(t => (decimal)t.Amount),
                    TransactionCount = g.Count()
                })
                .OrderBy(x => x.Date)
                .ToListAsync();
        }
    }
}
