using BusinessLayer.IService;
using DataAccessLayer.Entities;
using DataAccessLayer.IRepositories;
using DataAccessLayer.Repositories;
using Microsoft.EntityFrameworkCore;
using Wafi.SampleTest.Dtos;

namespace BusinessLayer.Service
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;

        public BookingService(IBookingRepository repository)
        {
            _bookingRepository = repository;
        }
        public async Task CreateAsync(Booking booking)
        {

            var bookingExist = await _bookingRepository.IsBookingExists(booking.CarId, booking.BookingDate, booking.StartTime, booking.EndTime, booking.RepeatOption, booking.DaysToRepeatOn, booking.EndRepeatDate);

            if (bookingExist)
 
            {
                throw new Exception("Already have a Booking. Select Another Date or Time");
            }

            await _bookingRepository.CreateAsync(booking);
        }

        public async Task<List<Booking>> GetBookings(BookingFilterDto input)
        {
            var bookings = await _bookingRepository.GetBookings(input);

            return bookings;
        }
    }
}
