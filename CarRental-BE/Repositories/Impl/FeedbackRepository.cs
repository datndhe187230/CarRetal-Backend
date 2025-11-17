using CarRental_BE.Models.Common;
using CarRental_BE.Models.DTO;
using Microsoft.EntityFrameworkCore;
using CarRental_BE.Models.NewEntities;
using CarRental_BE.Data;

namespace CarRental_BE.Repositories.Impl
{
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly CarRentalContext _context;

        public FeedbackRepository(CarRentalContext context)
        {
            _context = context;
        }

        public async Task<Review> AddFeedbackAsync(Review review)
        {
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public async Task<Booking> GetBookingAsync(string bookingNumber)
        {
            return await _context.Bookings
                .FirstOrDefaultAsync(b => b.BookingNumber == bookingNumber);
        }

        public async Task<FeedbackSummaryDTO> GetFeedbackSummaryByUserIdAsync(Guid userId)
        {
            var reviews = await _context.Reviews
                .Include(r => r.BookingNumberNavigation)
                .ThenInclude(b => b.Car)
                .Where(r => r.ToAccountId == userId)
                .ToListAsync();

            var summary = new FeedbackSummaryDTO
            {
                AverageRating = reviews.Any() ? Math.Round(reviews.Average(f => (int)f.Rating), 1) : 0,
                TotalRatings = reviews.Count,
                RatingDistribution = reviews
                    .GroupBy(f => (int)f.Rating)
                    .ToDictionary(g => g.Key, g => g.Count())
            };

            return summary;
        }

        public async Task<PaginationResponse<FeedbackItemDTO>> GetFeedbackItemsByUserIdAsync(Guid userId, PaginationRequest request)
        {
            var query = _context.Reviews
                .Include(r => r.BookingNumberNavigation)
                .ThenInclude(b => b.Car)
                .Where(r => r.ToAccountId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new FeedbackItemDTO
                {
                    CarName = r.BookingNumberNavigation.Car.Brand + " " + r.BookingNumberNavigation.Car.Model,
                    CarImageUrl = r.BookingNumberNavigation.Car.CarImages.Where(i => i.ImageType == "right").Select(i => i.Uri).FirstOrDefault(),
                    Comment = r.Comment,
                    Rating = r.Rating,
                    CreatedAt = r.CreatedAt,
                    PickUpTime = r.BookingNumberNavigation.PickUpTime,
                    DropOffTime = r.BookingNumberNavigation.DropOffTime,
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