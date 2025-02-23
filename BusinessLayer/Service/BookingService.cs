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
            var bookingExist = await _bookingRepository.IsBookingExistsAsync(booking.CarId, booking.BookingDate, booking.StartTime, booking.EndTime, booking.RepeatOption);

            if (bookingExist)
            {
                throw new Exception("Already have a Booking. Select Another Date or Time");
            }

            if (booking.BookingDate >= booking.EndRepeatDate || booking.StartTime >= booking.EndTime)
            {
                throw new Exception("Start time must be less than End Repeat Date or Time.");
            }

            if (booking.RepeatOption == RepeatOption.DoesNotRepeat)
            {
                if (booking.EndRepeatDate != null || booking.DaysToRepeatOn != null)
                {
                    throw new Exception("For a Single Booking, End Repeat Date and Repeat Days must be null.");
                }
            }
            
            if (booking.RepeatOption == RepeatOption.Daily)
            {
                if (booking.EndRepeatDate == null || booking.DaysToRepeatOn != null)
                {
                    throw new Exception("For Daily Repeat Booking, Must have a End Repeat Date and Repeat Days must be null.");
                }
            }

            if (booking.RepeatOption == RepeatOption.Weekly)
            {
                if (booking.EndRepeatDate == null || booking.DaysToRepeatOn == null)
                {
                    throw new Exception("For Weekly Repeat Booking, Must have a End Repeat Date and Mentioned Repeat Days.");
                }
            }

            await _bookingRepository.CreateAsync(booking);
        }

        public async Task<List<Booking>> GetBookingsAsync(BookingFilterDto input)
        {
            var bookings = await _bookingRepository.GetBookingsAsync(input);

            return bookings;
        }

        public List<BookingCalendarDto> ConvertToCalendarView(List<Booking> bookings)
        {
            var bookingList = new List<BookingCalendarDto>();

            foreach (var booking in bookings)
            {
                int repeatDays = 1;

                if (booking.RepeatOption == RepeatOption.Weekly) repeatDays = 7;
                else if (booking.RepeatOption == RepeatOption.DoesNotRepeat) booking.EndRepeatDate = booking.BookingDate;

                for (var i = booking.BookingDate; i <= booking.EndRepeatDate; i = i.AddDays(repeatDays))
                {
                    var calendarView = new BookingCalendarDto()
                    {
                        BookingDate = booking.BookingDate,
                        StartTime = booking.StartTime,
                        EndTime = booking.EndTime,
                        CarModel = booking.Car.Model
                    };

                     bookingList.Add(calendarView);
                }
            }

            return bookingList;
        }
    }
}
