using CarRental_BE.Models.VO.Car;
using CarRental_BE.Models.VO.User;

namespace CarRental_BE.Models.VO.Booking
{
    public record BookingInformationVO
    {
        public CarVO_CarDetail Car { get; set; }
        public UserProfileVO User { get; set; }
    }
}
