using Booking.DataAccess.Interfaces;
using Booking.Entities;
using Microsoft.EntityFrameworkCore;

namespace Booking.DataAccess.Repositories;

public class UserRepository(AppDbContext context) : IUserRepository
{
    public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken ct)
        => await context.Users.AsNoTracking().OrderBy(u => u.Name).ToListAsync(ct);

    public async Task<User?> GetByIdAsync(int id, CancellationToken ct)
        => await context.Users.FindAsync([id], ct);

    public Task<bool> ExistsAsync(int id, CancellationToken ct)
        => context.Users.AnyAsync(u => u.Id == id, ct);

    public Task<bool> EmailExistsAsync(string email, int? exceptId, CancellationToken ct)
        => context.Users.AnyAsync(u => u.Email == email && u.Id != exceptId, ct);

    public Task<bool> HasReservationsAsync(int id, CancellationToken ct)
        => context.Reservations.AnyAsync(r => r.UserId == id, ct);

    public void Add(User user) => context.Users.Add(user);

    public void Remove(User user) => context.Users.Remove(user);

    public Task SaveChangesAsync(CancellationToken ct) => context.SaveChangesAsync(ct);
}
