using Booking.DataAccess.Interfaces;
using Booking.Entities;
using Microsoft.EntityFrameworkCore;

namespace Booking.DataAccess.Repositories;

public class ReservationRepository(AppDbContext context) : IReservationRepository
{
    public async Task<IReadOnlyList<Reservation>> GetAllAsync(CancellationToken ct)
        => await context.Reservations.AsNoTracking().OrderBy(r => r.StartsAt).ToListAsync(ct);

    public async Task<IReadOnlyList<Reservation>> GetByRoomAsync(int roomId, CancellationToken ct)
        => await context.Reservations.AsNoTracking()
            .Where(r => r.RoomId == roomId)
            .OrderBy(r => r.StartsAt)
            .ToListAsync(ct);

    public async Task<Reservation?> GetByIdAsync(int id, CancellationToken ct)
        => await context.Reservations.FindAsync([id], ct);

    // deux créneaux se chevauchent si chacun commence avant que l'autre finisse
    public Task<bool> OverlapsAsync(int roomId, DateTime startsAt, DateTime endsAt, CancellationToken ct)
        => context.Reservations.AnyAsync(r =>
            r.RoomId == roomId && r.StartsAt < endsAt && startsAt < r.EndsAt, ct);

    public void Add(Reservation reservation) => context.Reservations.Add(reservation);

    public void Remove(Reservation reservation) => context.Reservations.Remove(reservation);

    public Task SaveChangesAsync(CancellationToken ct) => context.SaveChangesAsync(ct);
}
