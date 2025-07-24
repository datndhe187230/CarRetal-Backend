using CarRental_BE.Models.Entities;

namespace CarRental_BE.Repositories
{
    public interface IFeedbackRepository
    {
        Task<Feedback> AddFeedbackAsync(Feedback feedback);
        Task<Booking> GetBookingAsync(string bookingNumber);
    
    }
}
