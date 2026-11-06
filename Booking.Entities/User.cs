namespace Booking.Entities
{   // Represents a user in the booking system
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public List<Booking> Bookings { get; set; } // Navigation property for the user's bookings

    }
}   