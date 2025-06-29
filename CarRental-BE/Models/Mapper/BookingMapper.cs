using CarRental_BE.Models.Entities;
using CarRental_BE.Models.VO;
public static class BookingMapper
{

    public static BookingVO ToBookingVO(Booking booking)
    {
        var numberOfDays = (booking.DropOffTime - booking.PickUpTime)?.Days ?? 0;
        var totalPrice = numberOfDays * (booking.BasePrice ?? 0);

        return new BookingVO
        {
            BookingNumber = booking.BookingNumber,
            CarName = $"{booking.Car?.Brand} {booking.Car?.Model}" ?? "Unknown",
            PickupDate = booking.PickUpTime,
            ReturnDate = booking.DropOffTime,
            PickUpLocation = booking.PickUpLocation,
            DropOffLocation = booking.DropOffLocation,
            BasePrice = booking.BasePrice,
            Deposit = booking.Deposit,
            TotalPrice = totalPrice,
            NumberOfDays = numberOfDays,
            PaymentType = booking.PaymentType,
            Status = booking.Status,
            CreatedAt = booking.CreatedAt,
            CarImageUrl = booking.Car?.CarImageFront        // hoặc chọn ảnh khác
        };
    }
}
