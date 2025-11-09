using CarRental_BE.Models.VO.CarOwnerDashboard;
using CarRental_BE.Models.VO.Statistic;
using CarRental_BE.Repositories;
using CarRental_BE.Models.Enum;
using CarRental_BE.Models.Common;
using Microsoft.EntityFrameworkCore;
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
            // Fetch all bookings for cars owned by owner
            var all = await _bookingRepository.GetAllBookingsAsync();
            var list = all.Where(b => b.Car.AccountId == ownerAccountId).AsQueryable();

            // Filters
            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var term = query.Search.Trim().ToLower();
                list = list.Where(b =>
                (b.BookingNumber != null && b.BookingNumber.ToLower().Contains(term)) ||
                (b.DriverFullName != null && b.DriverFullName.ToLower().Contains(term)) ||
                (b.DriverEmail != null && b.DriverEmail.ToLower().Contains(term)) ||
                (b.DriverPhoneNumber != null && b.DriverPhoneNumber.ToLower().Contains(term)));
            }
            if (!string.IsNullOrWhiteSpace(query.CarName))
            {
                var carTerm = query.CarName.Trim().ToLower();
                list = list.Where(b => (b.Car.Brand + " " + b.Car.Model).ToLower().Contains(carTerm) || b.Car.Model.ToLower().Contains(carTerm));
            }
            if (query.Status != null && query.Status.Any())
            {
                var allowed = new HashSet<string>(Enum.GetNames(typeof(BookingStatusEnum)));
                var wanted = query.Status.Select(s => s.Trim().ToLower()).ToList();
                foreach (var s in wanted)
                {
                    if (!allowed.Contains(s))
                    {
                        throw new ArgumentException("Invalid status value: " + s);
                    }
                }
                list = list.Where(b => wanted.Contains(b.Status!.ToLower()));
            }
            if (query.FromDate.HasValue || query.ToDate.HasValue)
            {
                var from = query.FromDate?.Date ?? DateTime.MinValue.Date;
                var to = (query.ToDate?.Date ?? DateTime.MaxValue.Date).AddDays(1).AddTicks(-1);
                list = list.Where(b =>
                (b.PickUpTime.HasValue && b.DropOffTime.HasValue) &&
                b.PickUpTime.Value <= to && b.DropOffTime.Value >= from);
            }

            // Sorting
            var sortBy = (query.SortBy ?? string.Empty).ToLower();
            var sortDir = (query.SortDirection ?? "desc").ToLower();
            bool asc = sortDir == "asc";
            IOrderedQueryable<CarRental_BE.Models.Entities.Booking>? ordered = null;
            if (sortBy == "pickupdate")
            {
                ordered = asc ? list.OrderBy(b => b.PickUpTime) : list.OrderByDescending(b => b.PickUpTime);
            }
            else if (sortBy == "returndate")
            {
                ordered = asc ? list.OrderBy(b => b.DropOffTime) : list.OrderByDescending(b => b.DropOffTime);
            }
            else if (sortBy == "totalamount")
            {
                ordered = asc ? list.OrderBy(b => (b.BasePrice ?? 0) + (b.Deposit ?? 0)) : list.OrderByDescending(b => (b.BasePrice ?? 0) + (b.Deposit ?? 0));
            }
            else if (sortBy == "status")
            {
                ordered = asc ? list.OrderBy(b => b.Status) : list.OrderByDescending(b => b.Status);
            }
            else
            {
                // Default priority order: confirmed, in_progress, pending_payment, pending_deposit, then others; within group pickupDate DESC
                ordered = list
                .OrderBy(b => b.Status == BookingStatusEnum.confirmed.ToString() ? 0
                : b.Status == BookingStatusEnum.in_progress.ToString() ? 1
                : b.Status == BookingStatusEnum.pending_payment.ToString() ? 2
                : b.Status == BookingStatusEnum.pending_deposit.ToString() ? 3
                : 4)
                .ThenByDescending(b => b.PickUpTime);
            }

            var page = query.Page < 1 ? 1 : query.Page;
            var pageSize = query.PageSize < 1 ? 10 : query.PageSize > 100 ? 100 : query.PageSize;
            var total = await Task.FromResult(ordered.Count());
            var items = ordered.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var vo = items.Select(b => new CarOwnerBookingListItemVO
            {
                BookingNumber = b.BookingNumber,
                BookingId = b.BookingNumber,
                CarId = b.CarId,
                CarName = $"{b.Car.Brand} {b.Car.Model} {b.Car.ProductionYear}",
                CarImageFront = b.Car.CarImageFront,
                Status = b.Status ?? string.Empty,
                PickupDate = b.PickUpTime,
                ReturnDate = b.DropOffTime,
                PickUpLocation = b.PickUpLocation,
                DropOffLocation = b.DropOffLocation,
                BasePrice = b.BasePrice,
                Deposit = b.Deposit,
                TotalAmount = (b.BasePrice ?? 0) + (b.Deposit ?? 0),
                PaymentType = b.PaymentType,
                PaymentStatus = b.Transactions.Any() ? b.Transactions.All(t => t.Status == "Successful") ? "paid" : "partial" : null,
                CreatedAt = b.CreatedAt,
                UpdatedAt = b.UpdatedAt,
                RenterFullName = b.DriverFullName,
                RenterEmail = b.DriverEmail,
                RenterPhoneNumber = b.DriverPhoneNumber
            }).ToList();

            return new PaginationResponse<CarOwnerBookingListItemVO>(vo, page, pageSize, total);
        }
    }
}
