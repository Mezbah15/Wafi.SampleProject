using DataAccessLayer.Entities;
using Wafi.SampleTest.Dtos;

namespace BusinessLayer.IService
{
    public interface IBookingService
    {
        Task CreateAsync(Booking booking);
        Task<List<Booking>> GetBookings(BookingFilterDto input);
    }  
}
