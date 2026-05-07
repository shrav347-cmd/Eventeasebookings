using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventeaseBookingSystem.Data;
using EventeaseBookingSystem.Models;
using EventeaseBookingSystem.Services;

namespace EventeaseBookingSystem.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Events
        public async Task<IActionResult> Index()
        {
            var events = _context.Events.Include(e => e.Venue);
            return View(await events.ToListAsync());
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .Include(e => e.Venue)
                .FirstOrDefaultAsync(m => m.EventID == id);

            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // GET: Events/Create
        public IActionResult Create()
        {
            ViewData["VenueID"] = new SelectList(_context.Venues, "VenueID", "VenueName");
            return View();
        }

        // POST: Events/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("EventID,EventName,StartDate,EndDate,VenueID,ImageURL")] Event @event,
            IFormFile imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                var blobService = new BlobService();
                string imageUrl = await blobService.UploadFileAsync(imageFile);
                @event.ImageURL = imageUrl;

                ModelState.Remove("ImageURL");
            }
            else
            {
                ModelState.AddModelError("", "Please select an event image.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(@event);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Event created successfully.";
                return RedirectToAction(nameof(Index));
            }

            ViewData["VenueID"] = new SelectList(_context.Venues, "VenueID", "VenueName", @event.VenueID);
            return View(@event);
        }

        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events.FindAsync(id);

            if (@event == null)
            {
                return NotFound();
            }

            ViewData["VenueID"] = new SelectList(_context.Venues, "VenueID", "VenueName", @event.VenueID);
            return View(@event);
        }

        // POST: Events/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("EventID,EventName,StartDate,EndDate,VenueID,ImageURL")] Event @event,
            IFormFile imageFile)
        {
            if (id != @event.EventID)
            {
                return NotFound();
            }

            if (imageFile != null && imageFile.Length > 0)
            {
                var blobService = new BlobService();
                string imageUrl = await blobService.UploadFileAsync(imageFile);
                @event.ImageURL = imageUrl;

                ModelState.Remove("ImageURL");
            }
            else
            {
                var existingEvent = await _context.Events
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => e.EventID == id);

                if (existingEvent != null)
                {
                    @event.ImageURL = existingEvent.ImageURL;
                    ModelState.Remove("ImageURL");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@event);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Event updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.EventID))
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

            ViewData["VenueID"] = new SelectList(_context.Venues, "VenueID", "VenueName", @event.VenueID);
            return View(@event);
        }

        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .Include(e => e.Venue)
                .FirstOrDefaultAsync(m => m.EventID == id);

            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Events.FindAsync(id);

            if (@event == null)
            {
                return NotFound();
            }

            bool hasBookings = await _context.Bookings
                .AnyAsync(b => b.EventID == id);

            if (hasBookings)
            {
                TempData["ErrorMessage"] = "This event cannot be deleted because it has existing bookings.";
                return RedirectToAction(nameof(Index));
            }

            _context.Events.Remove(@event);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Event deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.EventID == id);
        }
    }
}