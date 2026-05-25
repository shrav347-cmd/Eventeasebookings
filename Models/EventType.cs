using System.Collections.Generic;

namespace EventeaseBookingSystem.Models
{
    public class EventType
    {
        public int EventTypeID { get; set; }

        public string EventTypeName { get; set; } = string.Empty;

        public ICollection<Event> Events { get; set; } = new List<Event>();
    }
}