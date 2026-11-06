using Booking.Entities;

namespace Booking.DataAccess.Interfaces;

public interface IRoomRepository
{
    Task<IReadOnlyList<Room>> GetAllAsync(CancellationToken ct);
    Task<Room?> GetByIdAsync(int id, CancellationToken ct);
    Task<IReadOnlyList<Room>> GetAvailableAsync(DateTime startsAt, DateTime endsAt, CancellationToken ct);
    Task<bool> NameExistsAsync(string name, int? exceptId, CancellationToken ct);
    Task<bool> HasReservationsAsync(int id, CancellationToken ct);
    void Add(Room room);
    void Remove(Room room);
    Task SaveChangesAsync(CancellationToken ct);
}
