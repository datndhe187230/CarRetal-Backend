using CarRental_BE.Data;
using CarRental_BE.Models.Common;
using CarRental_BE.Models.DTO;
using CarRental_BE.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarRental_BE.Repositories.Impl
{
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly CarRentalContext _context;

        public FeedbackRepository(CarRentalContext context)
        {
            _context = context;
        }

        public async Task<Feedback> AddFeedbackAsync(Feedback feedback)
        {
            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();
            return feedback;
        }

        public async Task<Booking> GetBookingAsync(string bookingNumber)
        {
            return await _context.Bookings
                .FirstOrDefaultAsync(b => b.BookingNumber == bookingNumber);
        }

        public async Task<FeedbackSummaryDTO> GetFeedbackSummaryByUserIdAsync(Guid userId)
        {
            var feedbacks = await _context.Feedbacks
                .Include(f => f.BookingNumberNavigation)
                .ThenInclude(b => b.Car)
                .Where(f => f.BookingNumberNavigation != null && f.BookingNumberNavigation.Car != null && f.BookingNumberNavigation.Car.AccountId == userId)
                .ToListAsync();

            var summary = new FeedbackSummaryDTO
            {
                AverageRating = feedbacks.Any() ? Math.Round(feedbacks.Average(f => f.Rating ?? 0), 1) : 0,
                TotalRatings = feedbacks.Count,
                RatingDistribution = feedbacks
                    .GroupBy(f => f.Rating ?? 0)
                    .ToDictionary(g => g.Key, g => g.Count())
            };

            return summary;
        }

        public async Task<PaginationResponse<FeedbackItemDTO>> GetFeedbackItemsByUserIdAsync(Guid userId, PaginationRequest request)
        {
            var query = _context.Feedbacks
                .Include(f => f.BookingNumberNavigation)
                .ThenInclude(b => b.Car)
                .Where(f => f.BookingNumberNavigation != null && f.BookingNumberNavigation.Car != null && f.BookingNumberNavigation.Car.AccountId == userId)
                .OrderByDescending(f => f.CreateAt)
                .Select(f => new FeedbackItemDTO
                {
                    CarName = f.BookingNumberNavigation.Car.Brand + " " + f.BookingNumberNavigation.Car.Model,
                    CarImageUrl = f.BookingNumberNavigation.Car.CarImageRight,
                    Comment = f.Comment,
                    Rating = f.Rating.GetValueOrDefault(),
                    CreatedAt = f.CreateAt.GetValueOrDefault(),
                    PickUpTime = f.BookingNumberNavigation.PickUpTime.GetValueOrDefault(),
                    DropOffTime = f.BookingNumberNavigation.DropOffTime.GetValueOrDefault(),
                });

            var totalRecords = await query.CountAsync();
            var items = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return new PaginationResponse<FeedbackItemDTO>(items, totalRecords, request.PageNumber, request.PageSize);
        }

    }
}
