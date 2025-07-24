using CarRental_BE.Data;
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
    }
}
