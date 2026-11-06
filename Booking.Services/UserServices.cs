using Booking.DataAccess.Interfaces;
using Booking.Dto;
using Booking.Entities;
using Booking.Entities.Exceptions;

namespace Booking.Services;

public interface IUserService
{
    // Voir tous les utilisateurs
    Task<IReadOnlyList<UserResponse>> GetAllAsync(CancellationToken ct);

    // Voir un utilisateur par son Id
    Task<UserResponse> GetAsync(int id, CancellationToken ct);

    // Créer un utilisateur
    Task<UserResponse> CreateAsync(UserRequest request, CancellationToken ct);

    // Modifier un utilisateur
    Task<UserResponse> UpdateAsync(int id, UserRequest request, CancellationToken ct);

    // Supprimer un utilisateur
    Task DeleteAsync(int id, CancellationToken ct);
}

public class UserService(IUserRepository users) : IUserService
{
    // Voir tous les utilisateurs
    public async Task<IReadOnlyList<UserResponse>> GetAllAsync(CancellationToken ct)
        => (await users.GetAllAsync(ct)).Select(u => u.ToDto()).ToList();

    // Voir un utilisateur par son Id
    public async Task<UserResponse> GetAsync(int id, CancellationToken ct)
        => (await GetExistingAsync(id, ct)).ToDto();

    // Créer un utilisateur
    public async Task<UserResponse> CreateAsync(UserRequest request, CancellationToken ct)
    {
        var email = request.Email.ToLowerInvariant();

        if (await users.EmailExistsAsync(email, null, ct))
            throw new BusinessRuleException("Cet email est déjà utilisé.");

        var user = new User { Name = request.Name, Email = email };
        users.Add(user);
        await users.SaveChangesAsync(ct);
        return user.ToDto();
    }

    // Modifier un utilisateur
    public async Task<UserResponse> UpdateAsync(int id, UserRequest request, CancellationToken ct)
    {
        var user = await GetExistingAsync(id, ct);
        var email = request.Email.ToLowerInvariant();

        if (await users.EmailExistsAsync(email, id, ct))
            throw new BusinessRuleException("Cet email est déjà utilisé par un autre utilisateur.");

        user.Name = request.Name;
        user.Email = email;
        await users.SaveChangesAsync(ct);
        return user.ToDto();
    }

    // Supprimer un utilisateur
    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        var user = await GetExistingAsync(id, ct);

        if (await users.HasReservationsAsync(id, ct))
            throw new BusinessRuleException("Cet utilisateur a encore des réservations.");

        users.Remove(user);
        await users.SaveChangesAsync(ct);
    }

    // Si l'utilisateur n'existe pas, on lève une exception
    private async Task<User> GetExistingAsync(int id, CancellationToken ct)
        => await users.GetByIdAsync(id, ct)
           ?? throw new NotFoundException($"Utilisateur {id} introuvable.");
}
