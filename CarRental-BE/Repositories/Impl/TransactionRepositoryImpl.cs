﻿using CarRental_BE.Data;
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
            var grouped = await _context.Transactions
                .Where(t => t.Status == "Completed")
                .Select(t => new
                {
                    t.Amount,
                    WalletId = t.WalletId,
                    AccountId = t.Wallet.IdNavigation.Id,
                    Email = t.Wallet.IdNavigation.Email,
                    FullName = t.Wallet.IdNavigation.UserProfile.FullName,
                    Phone = t.Wallet.IdNavigation.UserProfile.PhoneNumber,
                    JoinYear = t.Wallet.IdNavigation.CreatedAt.Year,
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
                        TotalPayments = g.Sum(x => (decimal)x.Amount),
                        TotalBookings = g.Select(x => x.Bookings).Distinct().Count(),
                        LastBooking = lastBooking.HasValue
                            ? (DateTime.Now - lastBooking.Value).Days + " days ago"
                            : "Unknown",
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
