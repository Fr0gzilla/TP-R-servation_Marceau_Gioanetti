using Booking.Entities;

namespace Booking.DataAccess.Interfaces;

public interface IReservationRepository
{
    Task<IReadOnlyList<Reservation>> GetAllAsync(CancellationToken ct);
    Task<IReadOnlyList<Reservation>> GetByRoomAsync(int roomId, CancellationToken ct);
    Task<Reservation?> GetByIdAsync(int id, CancellationToken ct);
    Task<bool> OverlapsAsync(int roomId, DateTime startsAt, DateTime endsAt, CancellationToken ct);
    void Add(Reservation reservation);
    void Remove(Reservation reservation);
    Task SaveChangesAsync(CancellationToken ct);
}
