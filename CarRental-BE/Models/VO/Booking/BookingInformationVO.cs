using CarRental_BE.Models.VO.Car;
using CarRental_BE.Models.VO.User;

namespace CarRental_BE.Models.VO.Booking
{
    public record BookingInformationVO
    {
        public CarVO_CarDetail Car { get; set; }
        public UserProfileVO User { get; set; }

        public OccupiedDateRange[] CarCallendar {  get; set; }

    }
    public class OccupiedDateRange
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }

}
