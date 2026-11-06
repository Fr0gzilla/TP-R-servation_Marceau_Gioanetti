using Booking.DataAccess.Interfaces;
using Booking.Entities;
using Microsoft.EntityFrameworkCore;

namespace Booking.DataAccess.Repositories;

public class RoomRepository(AppDbContext context) : IRoomRepository
{
    public async Task<IReadOnlyList<Room>> GetAllAsync(CancellationToken ct)
        => await context.Rooms.AsNoTracking().OrderBy(r => r.Name).ToListAsync(ct);

    public async Task<Room?> GetByIdAsync(int id, CancellationToken ct)
        => await context.Rooms.FindAsync([id], ct);

    public Task<bool> NameExistsAsync(string name, CancellationToken ct)
        => context.Rooms.AnyAsync(r => r.Name == name, ct);

    public Task<bool> HasReservationsAsync(int id, CancellationToken ct)
        => context.Reservations.AnyAsync(r => r.RoomId == id, ct);

    public void Add(Room room) => context.Rooms.Add(room);

    public void Remove(Room room) => context.Rooms.Remove(room);

    public Task SaveChangesAsync(CancellationToken ct) => context.SaveChangesAsync(ct);
}
