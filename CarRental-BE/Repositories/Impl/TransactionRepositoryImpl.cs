using CarRental_BE.Models.VO.Statistic;
using Microsoft.EntityFrameworkCore;
using CarRental_BE.Data;

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
                .SumAsync(t => t.AmountCents);
        }

        public async Task<IEnumerable<TopPayingCustomerVO>> GetTopPayingCustomersAsync(int count = 5)
        {
            var grouped = await _context.Transactions
                .Where(t => t.Status == "Completed")
                .Select(t => new
                {
                    t.AmountCents,
                    WalletId = t.WalletId,
                    AccountId = t.Wallet.AccountId,
                    Email = t.Wallet.Account.Email,
                    FullName = t.Wallet.Account.UserProfile.FullName,
                    Phone = t.Wallet.Account.UserProfile.PhoneNumber,
                    JoinYear = t.Wallet.Account.CreatedAt.Year,
                    Bookings = t.BookingNumber,
                    CarName = t.BookingNumberNavigation.Car != null
                        ? t.BookingNumberNavigation.Car.Brand + " " + t.BookingNumberNavigation.Car.Model
                        : "Unknown",
                    LastBookingAt = t.CreatedAt,
                })
                .ToListAsync(); // Force evaluation before complex LINQ

            var result = grouped
                .GroupBy(x => new
                {
                    x.WalletId,
                    x.AccountId,
                    x.Email,
                    x.FullName,
                    x.Phone,
                    x.JoinYear
                })
                .Select(g =>
                {
                    var lastBooking = g.Max(x => x.LastBookingAt);
                    return new TopPayingCustomerVO
                    {
                        AccountId = g.Key.AccountId,
                        Email = g.Key.Email,
                        CustomerName = g.Key.FullName ?? "Unknown",
                        Phone = g.Key.Phone ?? "Unknown",
                        MemberSince = g.Key.JoinYear.ToString(),
                        TotalPayments = g.Sum(x => x.AmountCents),
                        TotalBookings = g.Select(x => x.Bookings).Distinct().Count(),
                        LastBooking = lastBooking != default(DateTime) ? (DateTime.Now - lastBooking).Days + " days ago" : "Unknown",
                        PreferredVehicle = g.FirstOrDefault()?.CarName ?? "Unknown"
                    };
                })
                .OrderByDescending(x => x.TotalPayments)
                .Take(count)
                .ToList();

            return result;
        }

        public async Task<IEnumerable<TransactionTypeCountVO>> GetTransactionTypeCountsAsync()
        {
            return await _context.Transactions
                .GroupBy(t => t.Type)
                .Select(g => new TransactionTypeCountVO
                {
                    Type = g.Key ?? "Unknown",
                    Count = g.Count(),
                    TotalAmount = g.Sum(t => t.AmountCents)
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<DailyTransactionVO>> GetDailyTransactionsAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Transactions
                .Where(t => t.CreatedAt >= startDate && t.CreatedAt <= endDate)
                .GroupBy(t => t.CreatedAt.Date)
                .Select(g => new DailyTransactionVO
                {
                    Date = g.Key,
                    TotalAmount = g.Sum(t => t.AmountCents),
                    TransactionCount = g.Count()
                })
                .OrderBy(x => x.Date)
                .ToListAsync();
        }
    }
}
