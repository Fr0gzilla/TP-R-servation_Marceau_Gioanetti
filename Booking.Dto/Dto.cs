using System.ComponentModel.DataAnnotations;

namespace Booking.Dto;

// ----- Users -----
public record UserRequest(
    [Required, StringLength(100)] string Name,
    [Required, EmailAddress] string Email);

public record UserResponse(int Id, string Name, string Email);

// ----- Rooms -----
public record RoomRequest(
    [Required, StringLength(100)] string Name,
    [Range(1, 500)] int Capacity);

public record RoomResponse(int Id, string Name, int Capacity);

// ----- Reservations -----
public record ReservationRequest(
    [Required] int? UserId,
    [Required] int? RoomId,
    [Required] DateTime? StartsAt,
    [Required] DateTime? EndsAt,
    [Range(1, 500)] int Attendees);

// ----- Reservations -----
public record ReservationResponse(
    int Id, int UserId, int RoomId, DateTime StartsAt, DateTime EndsAt, int Attendees);