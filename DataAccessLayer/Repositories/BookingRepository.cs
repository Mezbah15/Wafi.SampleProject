using DataAccessLayer.Entities;
using DataAccessLayer.IRepositories;
using Microsoft.EntityFrameworkCore;
using Wafi.SampleTest.Dtos;

namespace DataAccessLayer.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly WafiDbContext _context;

        public BookingRepository(WafiDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(Booking booking)
        {
            await _context.Bookings.AddAsync(booking);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsBookingExistsAsync(Guid carId, DateOnly bookingDate, TimeSpan startTime, TimeSpan endTime, RepeatOption repeatOption)
        {
            var bookings = await _context.Bookings.Where(x => x.CarId == carId && ((x.RepeatOption == RepeatOption.DoesNotRepeat && x.BookingDate == bookingDate) || (x.RepeatOption != RepeatOption.DoesNotRepeat && x.EndRepeatDate >= bookingDate))).ToListAsync();

            var IsExist = bookings.Any(x =>
               (x.RepeatOption == RepeatOption.DoesNotRepeat
                   && x.BookingDate == bookingDate
                   && x.EndTime > startTime
                   && x.StartTime < endTime)
               || (x.RepeatOption == RepeatOption.Daily
                   && x.BookingDate <= bookingDate 
                   && x.EndRepeatDate >= bookingDate 
                   && x.EndTime > startTime
                   && x.StartTime < endTime) 
               || (x.RepeatOption == RepeatOption.Weekly
                   && x.BookingDate.DayOfWeek == bookingDate.DayOfWeek 
                   && x.BookingDate <= bookingDate
                   && x.EndRepeatDate >= bookingDate
                   && x.EndTime > startTime
                   && x.StartTime < endTime) 
           );

           return IsExist;
        }

        public async Task<List<Booking>> GetBookingsAsync(BookingFilterDto input)
        {
            var bookings = await _context.Bookings.Where(b => b.CarId == input.CarId && b.BookingDate >= input.StartBookingDate && b.BookingDate <= input.EndBookingDate).Include(b => b.Car).ToListAsync();

            return bookings;
        }
    }
}
