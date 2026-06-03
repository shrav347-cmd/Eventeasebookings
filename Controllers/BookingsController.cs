using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventeaseBookingSystem.Data;
using EventeaseBookingSystem.Models;

namespace EventeaseBookingSystem.Controllers
{
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Bookings with advanced filtering
        public async Task<IActionResult> Index(
            string searchString,
            int? eventTypeId,
            int? venueId,
            DateTime? startDate,
            DateTime? endDate,
            string availability)
        {
            var bookings = _context.Bookings
                .Include(b => b.Event)
                    .ThenInclude(e => e.EventType)
                .Include(b => b.Venue)
                .AsQueryable();

            // Basic search: Booking ID, Event Name, Venue Name, Customer Name
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                bookings = bookings.Where(b =>
                    b.Event.EventName.Contains(searchString) ||
                    b.Venue.VenueName.Contains(searchString) ||
                    b.CustomerName.Contains(searchString) ||
                    b.BookingID.ToString().Contains(searchString)
                );
            }

            // Filter by Event Type
            if (eventTypeId.HasValue && eventTypeId.Value > 0)
            {
                bookings = bookings.Where(b => b.Event.EventTypeID == eventTypeId.Value);
            }

            // Filter by Venue
            if (venueId.HasValue && venueId.Value > 0)
            {
                bookings = bookings.Where(b => b.VenueID == venueId.Value);
            }

            // Filter by Date Range
            if (startDate.HasValue)
            {
                bookings = bookings.Where(b => b.StartDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                bookings = bookings.Where(b => b.EndDate <= endDate.Value);
            }

            // Venue availability check
            if (!string.IsNullOrWhiteSpace(availability) && availability == "Available")
            {
                if (venueId.HasValue && venueId.Value > 0 && startDate.HasValue && endDate.HasValue)
                {
                    bool venueIsBooked = await _context.Bookings.AnyAsync(b =>
                        b.VenueID == venueId.Value &&
                        startDate.Value < b.EndDate &&
                        endDate.Value > b.StartDate
                    );

                    if (!venueIsBooked)
                    {
                        TempData["SuccessMessage"] = "The selected venue is available for the selected date range.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "The selected venue is not available for the selected date range.";
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Please select a venue, start date, and end date to check availability.";
                }
            }

            ViewData["CurrentFilter"] = searchString;
            ViewData["SelectedEventTypeId"] = eventTypeId;
            ViewData["SelectedVenueId"] = venueId;
            ViewData["StartDate"] = startDate?.ToString("yyyy-MM-ddTHH:mm");
            ViewData["EndDate"] = endDate?.ToString("yyyy-MM-ddTHH:mm");
            ViewData["Availability"] = availability;

            ViewData["EventTypeID"] = new SelectList(_context.EventTypes, "EventTypeID", "EventTypeName", eventTypeId);
            ViewData["VenueID"] = new SelectList(_context.Venues, "VenueID", "VenueName", venueId);

            return View(await bookings.ToListAsync());
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Event)
                    .ThenInclude(e => e.EventType)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(m => m.BookingID == id);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // GET: Bookings/Create
        public IActionResult Create()
        {
            ViewData["VenueID"] = new SelectList(_context.Venues, "VenueID", "VenueName");
            ViewData["EventID"] = new SelectList(_context.Events, "EventID", "EventName");
            return View();
        }

        // POST: Bookings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookingID,VenueID,EventID,CustomerName,StartDate,EndDate")] Booking booking)
        {
            if (booking.StartDate >= booking.EndDate)
            {
                ModelState.AddModelError("", "The booking end date/time must be after the start date/time.");
            }

            bool overlappingBooking = await _context.Bookings.AnyAsync(b =>
                b.VenueID == booking.VenueID &&
                booking.StartDate < b.EndDate &&
                booking.EndDate > b.StartDate
            );

            if (overlappingBooking)
            {
                ModelState.AddModelError("", "This venue is already booked for the selected date/time.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(booking);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Booking created successfully.";
                return RedirectToAction(nameof(Index));
            }

            ViewData["VenueID"] = new SelectList(_context.Venues, "VenueID", "VenueName", booking.VenueID);
            ViewData["EventID"] = new SelectList(_context.Events, "EventID", "EventName", booking.EventID);

            return View(booking);
        }

        public async Task<IActionResult> Report()
        {
            var bookings = await _context.Bookings
                .Include(b => b.Venue)
                .Include(b => b.Event)
                    .ThenInclude(e => e.EventType)
                .ToListAsync();

            var bookingsPerVenue = bookings
                .GroupBy(b => b.Venue.VenueName)
                .Select(g => new ReportViewModel
                {
                    Name = g.Key,
                    Count = g.Count()
                })
                .ToList();

            var bookingsPerEvent = bookings
                .GroupBy(b => b.Event.EventName)
                .Select(g => new ReportViewModel
                {
                    Name = g.Key,
                    Count = g.Count()
                })
                .ToList();

            ViewBag.BookingsPerVenue = bookingsPerVenue;
            ViewBag.BookingsPerEvent = bookingsPerEvent;

            return View();
        }

        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Venue)
                .Include(b => b.Event)
                    .ThenInclude(e => e.EventType)
                .FirstOrDefaultAsync(b => b.BookingID == id);

            if (booking == null)
            {
                return NotFound();
            }

            ViewData["VenueID"] = new SelectList(_context.Venues, "VenueID", "VenueName", booking.VenueID);
            ViewData["EventID"] = new SelectList(_context.Events, "EventID", "EventName", booking.EventID);

            return View(booking);
        }

        // POST: Bookings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingID,VenueID,EventID,CustomerName,StartDate,EndDate")] Booking booking)
        {
            if (id != booking.BookingID)
            {
                return NotFound();
            }

            if (booking.StartDate >= booking.EndDate)
            {
                ModelState.AddModelError("", "The booking end date/time must be after the start date/time.");
            }

            bool overlappingBooking = await _context.Bookings.AnyAsync(b =>
                b.BookingID != booking.BookingID &&
                b.VenueID == booking.VenueID &&
                booking.StartDate < b.EndDate &&
                booking.EndDate > b.StartDate
            );

            if (overlappingBooking)
            {
                ModelState.AddModelError("", "This venue is already booked for the selected date/time.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Booking updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.BookingID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            var bookingWithDetails = await _context.Bookings
                .Include(b => b.Venue)
                .Include(b => b.Event)
                    .ThenInclude(e => e.EventType)
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.BookingID == booking.BookingID);

            if (bookingWithDetails != null)
            {
                booking.Venue = bookingWithDetails.Venue;
                booking.Event = bookingWithDetails.Event;
            }

            ViewData["VenueID"] = new SelectList(_context.Venues, "VenueID", "VenueName", booking.VenueID);
            ViewData["EventID"] = new SelectList(_context.Events, "EventID", "EventName", booking.EventID);

            return View(booking);
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Event)
                    .ThenInclude(e => e.EventType)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(m => m.BookingID == id);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);

            if (booking == null)
            {
                return NotFound();
            }

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Booking deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.BookingID == id);
        }
    }
}