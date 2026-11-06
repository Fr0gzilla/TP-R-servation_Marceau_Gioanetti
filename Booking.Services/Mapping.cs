using Booking.Dto;
using Booking.Entities;

namespace Booking.Services
{
    public static class Mapping
    {   // ----- Users -----
        public static UserResponse ToDto(this User user) =>
            new(user.Id, user.Name, user.Email);

        // ----- Rooms -----
        public static RoomResponse ToDto(this Room room) =>
            new(room.Id, room.Name, room.Capacity);
        
        // ----- Reservations -----
        public static ReservationResponse ToDto(this Reservation reservation) =>
            new(reservation.Id, reservation.UserId, reservation.RoomId,
                reservation.StartsAt, reservation.EndsAt, reservation.Attendees);
    }
}