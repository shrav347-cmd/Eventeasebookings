namespace EventeaseBookingSystem.Models
{
    public class Venue
    {
        public int VenueID { get; set; }

        public string VenueName { get; set; } = string.Empty;

        public string Location { get; set; } = string.Empty;

        public int Capacity { get; set; }

        public ICollection<Event> Events { get; set; } = new List<Event>();

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}