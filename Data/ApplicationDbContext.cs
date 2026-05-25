using Microsoft.EntityFrameworkCore;
using EventeaseBookingSystem.Models;

namespace EventeaseBookingSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Venue> Venues { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<EventType> EventTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed predefined event types for Part 3
            modelBuilder.Entity<EventType>().HasData(
                new EventType { EventTypeID = 1, EventTypeName = "Wedding" },
                new EventType { EventTypeID = 2, EventTypeName = "Conference" },
                new EventType { EventTypeID = 3, EventTypeName = "Birthday" },
                new EventType { EventTypeID = 4, EventTypeName = "Concert" },
                new EventType { EventTypeID = 5, EventTypeName = "Corporate" },
                new EventType { EventTypeID = 6, EventTypeName = "Religious" },
                new EventType { EventTypeID = 7, EventTypeName = "Other" }
            );

            // Prevent multiple cascade paths: Venue -> Event
            modelBuilder.Entity<Event>()
                .HasOne(e => e.Venue)
                .WithMany(v => v.Events)
                .HasForeignKey(e => e.VenueID)
                .OnDelete(DeleteBehavior.Restrict);

            // EventType -> Event
            modelBuilder.Entity<Event>()
                .HasOne(e => e.EventType)
                .WithMany(et => et.Events)
                .HasForeignKey(e => e.EventTypeID)
                .OnDelete(DeleteBehavior.Restrict);

            // Prevent multiple cascade paths: Event -> Booking
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Event)
                .WithMany(e => e.Bookings)
                .HasForeignKey(b => b.EventID)
                .OnDelete(DeleteBehavior.Restrict);

            // Prevent multiple cascade paths: Venue -> Booking
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Venue)
                .WithMany(v => v.Bookings)
                .HasForeignKey(b => b.VenueID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}