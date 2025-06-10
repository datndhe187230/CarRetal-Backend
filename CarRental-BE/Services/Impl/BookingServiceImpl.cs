using CarRental_BE.Models.Mapper;
using CarRental_BE.Models.VO;
using CarRental_BE.Repositories;
using CarRental_BE.Repositories.Impl;
using CarRental_BE.Services;

public class BookingServiceImpl : IBookingService
{
    private readonly IBookingRepository _bookingRepository;

    public BookingServiceImpl(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public async Task<List<BookingVO>> GetAllBookingsAsync()
    {
        var bookingEntities = await _bookingRepository.GetAllBookingsAsync();
        return bookingEntities.Select(BookingMapper.ToBookingVO).ToList();
    }
    public async Task<List<BookingVO>> GetBookingsByAccountIdAsync(Guid accountId)
    {
        var bookingEntities = await _bookingRepository.GetBookingsByAccountIdAsync(accountId);
        return bookingEntities.Select(BookingMapper.ToBookingVO).ToList();
    }
    public async Task<(List<BookingVO>, int)> GetBookingsWithPagingAsync(int page, int pageSize)
    {
        var (entities, totalCount) = await _bookingRepository.GetBookingsWithPagingAsync(page, pageSize);
        var voList = entities.Select(BookingMapper.ToBookingVO).ToList();
        return (voList, totalCount);
    }

}
