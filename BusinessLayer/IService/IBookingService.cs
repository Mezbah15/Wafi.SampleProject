using DataAccessLayer.Entities;
using Wafi.SampleTest.Dtos;

namespace BusinessLayer.IService
{
    public interface IBookingService
    {
        List<BookingCalendarDto> ConvertToCalendarView(List<Booking> bookings);
        Task CreateAsync(Booking booking);
        Task<List<Booking>> GetBookings(BookingFilterDto input);
    }  
}
