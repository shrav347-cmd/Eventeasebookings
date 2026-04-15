namespace EventeaseBookingSystem.Models
{
    public class Booking
    {
        public int BookingID { get; set; }

        public int VenueID { get; set; }
        public Venue? Venue { get; set; }   

        public int EventID { get; set; }
        public Event? Event { get; set; }   

        public string CustomerName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}