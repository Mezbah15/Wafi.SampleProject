using BusinessLayer.IService;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wafi.SampleTest.Dtos;

namespace Wafi.SampleTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService service)
        {
            _bookingService = service;
        }

        // GET: api/Bookings
        [HttpGet("Booking")]
        public async Task<IEnumerable<BookingCalendarDto>> GetCalendarBookings([FromQuery] BookingFilterDto input)
        {
            // Get booking from the database and filter the data
            var bookings = await _bookingService.GetBookings(input);


            // TO DO: convert the database bookings to calendar view (date, start time, end time). Consiser NoRepeat, Daily and Weekly options

            List<BookingCalendarDto> bookingList = _bookingService.ConvertToCalendarView(bookings);
            
            return bookingList;
        }

        // POST: api/Bookings
        [HttpPost("Booking")]
        public async Task<IActionResult> PostBooking(CreateUpdateBookingDto booking)
        {
            // TO DO: Validate if any booking time conflicts with existing data. Return error if any conflicts
            try
            {
                if (ModelState.IsValid)
                {
                    var bookings = new Booking()
                    {
                        BookingDate = booking.BookingDate,
                        CarId = booking.CarId,
                        StartTime = booking.StartTime,
                        EndTime = booking.EndTime,
                        DaysToRepeatOn = booking.DaysToRepeatOn,
                        EndRepeatDate = booking.EndRepeatDate,
                        RepeatOption = booking.RepeatOption,
                        RequestedOn = booking.RequestedOn
                    };
                    await _bookingService.CreateAsync(bookings);
                }

                return Ok(booking);
            }
            catch (Exception e)
            {

                return StatusCode(500, new { message = e.Message });
            }
        }

        //GET: api/SeedData
        //For test purpose
        [HttpGet("SeedData")]
        //public async Task<IEnumerable<BookingCalendarDto>> GetSeedData()
        //{
        //    var cars = await _bookingService.Cars.ToListAsync();

        //    if (!cars.Any())
        //    {
        //        cars = GetCars().ToList();
        //        await _bookingService.Cars.AddRangeAsync(cars);
        //        await _bookingService.SaveChangesAsync();
        //    }

        //    var bookings = await _bookingService.Bookings.ToListAsync();

        //    if (!bookings.Any())
        //    {
        //        bookings = GetBookings().ToList();

        //        await _bookingService.Bookings.AddRangeAsync(bookings);
        //        await _bookingService.SaveChangesAsync();
        //    }

        //    var calendar = new Dictionary<DateOnly, List<Booking>>();

        //    foreach (var booking in bookings)
        //    {
        //        var currentDate = booking.BookingDate;
        //        while (currentDate <= (booking.EndRepeatDate ?? booking.BookingDate))
        //        {
        //            if (!calendar.ContainsKey(currentDate))
        //                calendar[currentDate] = new List<Booking>();

        //            calendar[currentDate].Add(booking);

        //            currentDate = booking.RepeatOption switch
        //            {
        //                RepeatOption.Daily => currentDate.AddDays(1),
        //                RepeatOption.Weekly => currentDate.AddDays(7),
        //                _ => booking.EndRepeatDate.HasValue ? booking.EndRepeatDate.Value.AddDays(1) : currentDate.AddDays(1)
        //            };
        //        }
        //    }

        //    List<BookingCalendarDto> result = new List<BookingCalendarDto>();

        //    foreach (var item in calendar)
        //    {
        //        foreach (var booking in item.Value)
        //        {
        //            result.Add(new BookingCalendarDto { BookingDate = booking.BookingDate, CarModel = booking.Car.Model, StartTime = booking.StartTime, EndTime = booking.EndTime });
        //        }
        //    }

        //    return result;
        //}

        #region Sample Data

        private IList<Car> GetCars()
        {
            var cars = new List<Car>
            {
                new Car { Id = Guid.NewGuid(), Make = "Toyota", Model = "Corolla" },
                new Car { Id = Guid.NewGuid(), Make = "Honda", Model = "Civic" },
                new Car { Id = Guid.NewGuid(), Make = "Ford", Model = "Focus" }
            };

            return cars;
        }

        private IList<Booking> GetBookings()
        {
            var cars = GetCars();

            var bookings = new List<Booking>
            {
                new Booking { Id = Guid.NewGuid(), BookingDate = new DateOnly(2025, 2, 5), StartTime = new TimeSpan(10, 0, 0), EndTime = new TimeSpan(12, 0, 0), RepeatOption = RepeatOption.DoesNotRepeat, RequestedOn = DateTime.Now, CarId = cars[0].Id, Car = cars[0] },
                new Booking { Id = Guid.NewGuid(), BookingDate = new DateOnly(2025, 2, 10), StartTime = new TimeSpan(14, 0, 0), EndTime = new TimeSpan(16, 0, 0), RepeatOption = RepeatOption.Daily, EndRepeatDate = new DateOnly(2025, 2, 20), RequestedOn = DateTime.Now, CarId = cars[1].Id, Car = cars[1] },
                new Booking { Id = Guid.NewGuid(), BookingDate = new DateOnly(2025, 2, 15), StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(10, 30, 0), RepeatOption = RepeatOption.Weekly, EndRepeatDate = new DateOnly(2025, 3, 31), RequestedOn = DateTime.Now, DaysToRepeatOn = DaysOfWeek.Monday, CarId = cars[2].Id,  Car = cars[2] },
                new Booking { Id = Guid.NewGuid(), BookingDate = new DateOnly(2025, 3, 1), StartTime = new TimeSpan(11, 0, 0), EndTime = new TimeSpan(13, 0, 0), RepeatOption = RepeatOption.DoesNotRepeat, RequestedOn = DateTime.Now, CarId = cars[0].Id, Car = cars[0] },
                new Booking { Id = Guid.NewGuid(), BookingDate = new DateOnly(2025, 3, 7), StartTime = new TimeSpan(8, 0, 0), EndTime = new TimeSpan(10, 0, 0), RepeatOption = RepeatOption.Weekly, EndRepeatDate = new DateOnly(2025, 3, 28), RequestedOn = DateTime.Now, DaysToRepeatOn = DaysOfWeek.Friday, CarId = cars[1].Id, Car = cars[1] },
                new Booking { Id = Guid.NewGuid(), BookingDate = new DateOnly(2025, 3, 15), StartTime = new TimeSpan(15, 0, 0), EndTime = new TimeSpan(17, 0, 0), RepeatOption = RepeatOption.Daily, EndRepeatDate = new DateOnly(2025, 3, 20), RequestedOn = DateTime.Now, CarId = cars[2].Id,  Car = cars[2] }
            };

            return bookings;
        }

        #endregion

    }
}
