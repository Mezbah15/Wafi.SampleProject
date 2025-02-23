using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Entities;
using Wafi.SampleTest.Dtos;

namespace DataAccessLayer.IRepositories
{
    public interface IBookingRepository
    {
        Task<List<Booking>> GetBookingsAsync(BookingFilterDto input);
        Task CreateAsync(Booking booking);
        Task<bool> IsBookingExistsAsync(Guid carId, DateOnly bookingDate, TimeSpan startTime, TimeSpan endTime, RepeatOption repeatOption);
    }
}
