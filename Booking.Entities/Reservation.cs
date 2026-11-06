namespace Booking.Entities;

public class Reservation
{
    public int Id { get; set; }
    public DateTime StartsAt { get; set; }
    public DateTime EndsAt { get; set; }
    public int Attendees { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; }

    public int RoomId { get; set; }
    public Room? Room { get; set; }
}
