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

        public async Task<bool> IsBookingExists(Guid carId, DateOnly bookingDate, TimeSpan startTime, TimeSpan endTime, RepeatOption repeatOption, DaysOfWeek? days, DateOnly? EndRepeatDate)
        {

            if (repeatOption == RepeatOption.DoesNotRepeat)
            {
                var existingBookings = await _context.Bookings.AnyAsync(x => x.CarId == carId && x.BookingDate == bookingDate && x.EndTime > startTime && x.StartTime < endTime);

                return existingBookings;
            }

            else if (repeatOption == RepeatOption.Daily)
            {
                var existingBookings = await _context.Bookings.Where(x => x.CarId == carId &&
                ((x.RepeatOption == RepeatOption.Daily && x.BookingDate <= bookingDate && x.EndRepeatDate >= bookingDate && x.EndTime > startTime && x.StartTime < endTime)
                || (x.RepeatOption == RepeatOption.DoesNotRepeat && x.BookingDate == bookingDate && x.EndTime > startTime && x.StartTime < endTime))).ToListAsync();

                return existingBookings.Any();
            }

            return ;
        }

        public async Task<List<Booking>> GetBookings(BookingFilterDto input)
        {
            var bookings = await _context.Bookings.Where(b => b.CarId == input.CarId && b.BookingDate >= input.StartBookingDate && b.BookingDate <= input.EndBookingDate).Include(b => b.Car).ToListAsync();

            return bookings;
        }
    }
}
