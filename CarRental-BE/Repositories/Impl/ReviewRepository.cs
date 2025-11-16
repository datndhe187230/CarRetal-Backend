using CarRental_BE.Data;
using CarRental_BE.Models;
using CarRental_BE.Models.Common;
using CarRental_BE.Models.DTO;
using CarRental_BE.Models.NewEntities;
using Microsoft.EntityFrameworkCore;

namespace CarRental_BE.Repositories.Impl
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly CarRentalContext _context;

        public ReviewRepository(CarRentalContext context)
        {
            _context = context;
        }

        public async Task<Review> AddFeedbackAsync(Review feedback)
        {
            _context.Reviews.Add(feedback);
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
            var feedbacks = await _context.Reviews
                .Include(f => f.BookingNumberNavigation)
                .ThenInclude(b => b.Car)
                .Where(f => f.BookingNumberNavigation != null && f.BookingNumberNavigation.Car != null && f.BookingNumberNavigation.Car.OwnerAccountId == userId)
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
            var query = _context.Reviews
                .Include(f => f.BookingNumberNavigation)
                .ThenInclude(b => b.Car)
                .Where(f => f.BookingNumberNavigation != null && f.BookingNumberNavigation.Car != null && f.BookingNumberNavigation.Car.OwnerAccountId == userId)
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

            var totalPages = (int)Math.Ceiling((double)totalRecords / request.PageSize);

            var validPageNumber = request.PageNumber;
            if (validPageNumber > totalPages && totalPages > 0)
            {
                validPageNumber = totalPages;
            }
            if (validPageNumber < 1)
            {
                validPageNumber = 1;
            }

            var items = await query
                .Skip((validPageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return new PaginationResponse<FeedbackItemDTO>(items, totalRecords, validPageNumber, request.PageSize);
        }

    }
}
