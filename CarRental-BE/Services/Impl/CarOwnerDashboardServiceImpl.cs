using CarRental_BE.Models.VO.CarOwnerDashboard;
using CarRental_BE.Models.VO.Statistic;
using CarRental_BE.Repositories;
using CarRental_BE.Models.Enum;
using CarRental_BE.Models.Common;
using CarRental_BE.Models.DTO;

namespace CarRental_BE.Services.Impl
{
    public class CarOwnerDashboardServiceImpl : ICarOwnerDashboardService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ICarRepository _carRepository;

        public CarOwnerDashboardServiceImpl(
        IBookingRepository bookingRepository,
        IFeedbackRepository feedbackRepository,
        IAccountRepository accountRepository,
        ICarRepository carRepository)
        {
            _bookingRepository = bookingRepository;
            _feedbackRepository = feedbackRepository;
            _accountRepository = accountRepository;
            _carRepository = carRepository;
        }

        public async Task<CarOwnerDashboardStatsVO> GetStatsAsync(Guid ownerAccountId)
        {
            var totalRevenue = await _bookingRepository.GetOwnerTotalRevenueAsync(ownerAccountId);
            var activeBookings = await _bookingRepository.GetOwnerActiveBookingsCountAsync(ownerAccountId);
            var totalCustomers = await _bookingRepository.GetOwnerTotalCustomersCountAsync(ownerAccountId);
            var fleetUtilization = await _bookingRepository.GetOwnerFleetUtilizationAsync(ownerAccountId);

            // Placeholder trend strings, could be computed from previous period deltas
            return new CarOwnerDashboardStatsVO
            {
                TotalRevenue = totalRevenue,
                ActiveBookings = activeBookings,
                TotalCustomers = totalCustomers,
                FleetUtilization = fleetUtilization,
                RevenueChange = "+0.0%",
                BookingsChange = "+0.0%",
                CustomersChange = "+0.0%",
                UtilizationChange = "+0.0%"
            };
        }

        public async Task<IEnumerable<CarOwnerMonthlyRevenueVO>> GetMonthlyRevenueAsync(Guid ownerAccountId, int year)
        {
            var items = await _bookingRepository.GetOwnerMonthlyRevenueAsync(ownerAccountId, year);
            var monthNames = new[] { "", "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
            return items.Select(m => new CarOwnerMonthlyRevenueVO
            {
                Month = monthNames[int.Parse(m.Month)],
                Total = m.Total
            });
        }

        public async Task<CarOwnerRatingSummaryVO> GetRatingSummaryAsync(Guid ownerAccountId)
        {
            // Reuse feedback repository aggregate for owner's cars
            var summary = await _feedbackRepository.GetFeedbackSummaryByUserIdAsync(ownerAccountId);
            var distribution = new List<CarOwnerRatingDistributionItemVO>();
            int total = summary.TotalRatings;
            for (int stars = 5; stars >= 1; stars--)
            {
                summary.RatingDistribution.TryGetValue(stars, out var count);
                int percentage = total > 0 ? (int)Math.Round((count * 100.0) / total) : 0;
                distribution.Add(new CarOwnerRatingDistributionItemVO
                {
                    Stars = stars,
                    Count = count,
                    Percentage = percentage
                });
            }
            return new CarOwnerRatingSummaryVO
            {
                OverallRating = summary.AverageRating,
                TotalReviews = summary.TotalRatings,
                TrendChange = "+0.0%",
                Distribution = distribution
            };
        }

        public async Task<IEnumerable<CarOwnerRecentReviewVO>> GetRecentReviewsAsync(Guid ownerAccountId, int limit = 3)
        {
            var request = new CarRental_BE.Models.Common.PaginationRequest
            {
                PageNumber = 1,
                PageSize = limit
            };
            var items = await _feedbackRepository.GetFeedbackItemsByUserIdAsync(ownerAccountId, request);
            return items.Data.Select(i => new CarOwnerRecentReviewVO
            {
                Id = Guid.NewGuid().ToString(), // No id in DTO, generate ephemeral
                ReviewerName = i.CarName, // No reviewer name in current DTO; using car name as placeholder
                Comment = i.Comment ?? string.Empty,
                Rating = i.Rating,
                CreatedAt = i.CreatedAt
            });
        }

        public async Task<PaginationResponse<CarOwnerBookingListItemVO>> GetOwnerBookingsAsync(Guid ownerAccountId, CarOwnerBookingListDTO query)
        {
            // Use repository optimized filtered query
            var (items, total) = await _bookingRepository.GetOwnerBookingsFilteredAsync(ownerAccountId, query);

            var vo = items.Select(b => new CarOwnerBookingListItemVO
            {
                BookingNumber = b.BookingNumber,
                BookingId = b.BookingNumber,
                CarId = b.CarId,
                CarName = $"{b.Car.Brand} {b.Car.Model} {b.Car.ProductionYear}",
                CarImageFront = b.Car.CarImages.Where(i => i.ImageType == "front").Select(i => i.Uri).FirstOrDefault(),
                Status = b.Status ?? string.Empty,
                PickupDate = b.PickUpTime,
                ReturnDate = b.DropOffTime,
                PickUpLocation = b.PickUpAddress != null ? b.PickUpAddress.CityProvince : null,
                DropOffLocation = b.DropOffAddress != null ? b.DropOffAddress.CityProvince : null,
                BasePrice = (long?)(b.BasePriceSnapshotCents ?? 0),
                Deposit = (long?)(b.DepositSnapshotCents ?? 0),
                TotalAmount = (long?)((b.BasePriceSnapshotCents ?? 0) + (b.DepositSnapshotCents ?? 0)),
                PaymentType = b.PaymentMethod,
                PaymentStatus = b.Transactions.Any() ? (b.Transactions.All(t => t.Status == "Successful") ? "paid" : "partial") : null,
                CreatedAt = b.CreatedAt,
                UpdatedAt = b.UpdatedAt,
                RenterFullName = b.BookingDrivers.FirstOrDefault() != null ? b.BookingDrivers.First().FullName : null,
                RenterEmail = b.RenterAccount.Email,
                RenterPhoneNumber = b.BookingDrivers.FirstOrDefault() != null ? b.BookingDrivers.First().PhoneNumber : null
            }).ToList();

            return new PaginationResponse<CarOwnerBookingListItemVO>(vo, query.Page, query.PageSize, total);
        }

        public async Task<List<BookingVO>?> GetUpcommingBookingsByAccountIdAsync(Guid accountId, int limit)
        {
            // Limit filtering server-side first then map
            var filterDto = new CarOwnerBookingListDTO
            {
                Page = 1,
                PageSize = limit,
                Status = new List<string> { BookingStatusEnum.in_progress.ToString(), BookingStatusEnum.pending_deposit.ToString(), BookingStatusEnum.confirmed.ToString() }
            };
            var (items, _) = await _bookingRepository.GetOwnerBookingsFilteredAsync(accountId, filterDto);
            return items.Select(BookingMapper.ToBookingVO).Take(limit).ToList();
        }

        public async Task<CarOwnerEarningsVO> GetEarningsAsync(Guid ownerAccountId)
        {
            // Pull only completed bookings for metrics and optionally minimal others
            var metricsQuery = new CarOwnerBookingListDTO
            {
                Status = new List<string> { BookingStatusEnum.completed.ToString() },
                Page = 1,
                PageSize = 10000 // large page to aggregate if needed
            };
            var (completedBookings, totalCompletedCount) = await _bookingRepository.GetOwnerBookingsFilteredAsync(ownerAccountId, metricsQuery);
            if (totalCompletedCount == 0)
            {
                return new CarOwnerEarningsVO
                {
                    TotalRevenue = 0,
                    RevenueChange = string.Empty,
                    NetProfit = 0,
                    NetProfitChange = string.Empty,
                    PendingPayouts = 0,
                    AverageBookingValue = 0,
                    AverageBookingValueChange = string.Empty,
                    CompletedBookingsThisMonth = 0,
                    MonthlyRevenue = new List<CarOwnerMonthlyRevenuePoint>()
                };
            }

            long totalRevenue = (long)completedBookings.Sum(b => b.BasePriceSnapshotCents ?? 0);
            long completedCountThisMonth = completedBookings.Count(b => b.CreatedAt.Year == DateTime.UtcNow.Year && b.CreatedAt.Month == DateTime.UtcNow.Month);
            long avgBookingValue = totalCompletedCount > 0 ? totalRevenue / totalCompletedCount : 0;
            long pendingPayouts = (long)completedBookings.SelectMany(b => b.Transactions).Where(t => t.Type == "receive_deposit" && t.Status != "Successful").Sum(t => t.AmountCents);

            var monthly = completedBookings
            .GroupBy(b => new { b.CreatedAt.Year, b.CreatedAt.Month })
            .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
            .Select(g => new CarOwnerMonthlyRevenuePoint
            {
                Month = $"{g.Key.Year:D4}-{g.Key.Month:D2}",
                Total = (long)g.Sum(b => b.BasePriceSnapshotCents ?? 0)
            }).ToList();

            return new CarOwnerEarningsVO
            {
                TotalRevenue = totalRevenue,
                RevenueChange = string.Empty,
                NetProfit = totalRevenue, // placeholder same as revenue
                NetProfitChange = string.Empty,
                PendingPayouts = pendingPayouts,
                AverageBookingValue = avgBookingValue,
                AverageBookingValueChange = string.Empty,
                CompletedBookingsThisMonth = (int)completedCountThisMonth,
                MonthlyRevenue = monthly
            };
        }

        public async Task<CarOwnerFleetVO> GetFleetAsync(Guid ownerAccountId)
        {
            var query = new CarOwnerBookingListDTO
            {
                Page = 1,
                PageSize = 10000
            };
            var (bookings, total) = await _bookingRepository.GetOwnerBookingsFilteredAsync(ownerAccountId, query);
            var ownerCars = await _carRepository.GetAccountId(ownerAccountId, 1, int.MaxValue);
            var carList = ownerCars.cars;

            if (carList == null || carList.Count == 0)
            {
                return new CarOwnerFleetVO
                {
                    FleetUtilization = 0,
                    UtilizationChange = string.Empty,
                    TotalBookings = 0,
                    BookingsChange = string.Empty,
                    AverageBookingDurationDays = 0,
                    AverageBookingDurationChange = string.Empty,
                    UpcomingBookings = 0,
                    CancellationRate = 0,
                    CancellationRateChange = string.Empty,
                    ActiveFleet = 0,
                    InactiveFleet = 0,
                    FleetSize = 0
                };
            }

            int fleetSize = carList.Count;
            int activeFleet = carList.Count(c => c.Status?.ToLower() == "verified");
            int inactiveFleet = fleetSize - activeFleet;

            int totalBookings = total;
            double avgDurationDays = bookings
            .Select(b => (b.DropOffTime - b.PickUpTime).TotalDays)
            .DefaultIfEmpty(0)
            .Average();
            int upcoming = bookings.Count(b => b.Status == BookingStatusEnum.confirmed.ToString() || b.Status == BookingStatusEnum.pending_deposit.ToString() || b.Status == BookingStatusEnum.in_progress.ToString());
            int cancelled = bookings.Count(b => b.Status == BookingStatusEnum.cancelled.ToString());
            decimal cancellationRate = totalBookings > 0 ? (decimal)cancelled / totalBookings : 0m;
            decimal utilization = activeFleet > 0 ? Math.Round(((decimal)upcoming / activeFleet), 2) : 0m;

            return new CarOwnerFleetVO
            {
                FleetUtilization = utilization,
                UtilizationChange = string.Empty,
                TotalBookings = totalBookings,
                BookingsChange = string.Empty,
                AverageBookingDurationDays = Math.Round(avgDurationDays, 1),
                AverageBookingDurationChange = string.Empty,
                UpcomingBookings = upcoming,
                CancellationRate = Math.Round(cancellationRate, 3),
                CancellationRateChange = string.Empty,
                ActiveFleet = activeFleet,
                InactiveFleet = inactiveFleet,
                FleetSize = fleetSize
            };
        }
    }
}
