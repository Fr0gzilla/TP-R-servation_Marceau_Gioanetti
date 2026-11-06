using Booking.Entities;

namespace Booking.DataAccess.Interfaces;

public interface IUserRepository
{
    Task<IReadOnlyList<User>> GetAllAsync(CancellationToken ct);
    Task<User?> GetByIdAsync(int id, CancellationToken ct);
    Task<bool> ExistsAsync(int id, CancellationToken ct);
    Task<bool> EmailExistsAsync(string email, int? exceptId, CancellationToken ct);
    Task<bool> HasReservationsAsync(int id, CancellationToken ct);
    void Add(User user);
    void Remove(User user);
    Task SaveChangesAsync(CancellationToken ct);
}
