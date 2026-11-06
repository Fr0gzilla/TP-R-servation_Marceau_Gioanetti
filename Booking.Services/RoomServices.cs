using Booking.DataAccess.Interfaces;
using Booking.Dto;
using Booking.Entities;
using Booking.Entities.Exceptions;

namespace Booking.Services;

public interface IRoomService
{
    Task<IReadOnlyList<RoomResponse>> GetAllAsync(CancellationToken ct);
    Task<RoomResponse> GetAsync(int id, CancellationToken ct);
    Task<IReadOnlyList<RoomResponse>> GetAvailableAsync(DateTime startsAt, DateTime endsAt, CancellationToken ct);
    Task<IReadOnlyList<ReservationResponse>> GetReservationsAsync(int id, CancellationToken ct);
    Task<RoomResponse> CreateAsync(RoomRequest request, CancellationToken ct);
    Task<RoomResponse> UpdateAsync(int id, RoomRequest request, CancellationToken ct);
    Task DeleteAsync(int id, CancellationToken ct);
}

public class RoomService(IRoomRepository rooms, IReservationRepository reservations) : IRoomService
{
    public async Task<IReadOnlyList<RoomResponse>> GetAllAsync(CancellationToken ct)
        => (await rooms.GetAllAsync(ct)).Select(r => r.ToDto()).ToList();

    public async Task<RoomResponse> GetAsync(int id, CancellationToken ct)
        => (await GetExistingAsync(id, ct)).ToDto();

    // les salles libres sur un créneau
    public async Task<IReadOnlyList<RoomResponse>> GetAvailableAsync(DateTime startsAt, DateTime endsAt, CancellationToken ct)
    {
        if (startsAt >= endsAt)
            throw new ArgumentException("La date de début doit être avant la date de fin.");

        return (await rooms.GetAvailableAsync(startsAt, endsAt, ct)).Select(r => r.ToDto()).ToList();
    }

    // le planning d'une salle
    public async Task<IReadOnlyList<ReservationResponse>> GetReservationsAsync(int id, CancellationToken ct)
    {
        await GetExistingAsync(id, ct);   // 404 si la salle n'existe pas
        return (await reservations.GetByRoomAsync(id, ct)).Select(r => r.ToDto()).ToList();
    }

    public async Task<RoomResponse> CreateAsync(RoomRequest request, CancellationToken ct)
    {
        if (await rooms.NameExistsAsync(request.Name, null, ct))
            throw new BusinessRuleException("Une salle porte déjà ce nom.");

        var room = new Room { Name = request.Name, Capacity = request.Capacity };
        rooms.Add(room);
        await rooms.SaveChangesAsync(ct);
        return room.ToDto();
    }

    public async Task<RoomResponse> UpdateAsync(int id, RoomRequest request, CancellationToken ct)
    {
        var room = await GetExistingAsync(id, ct);

        if (await rooms.NameExistsAsync(request.Name, id, ct))
            throw new BusinessRuleException("Une autre salle porte déjà ce nom.");

        room.Name = request.Name;
        room.Capacity = request.Capacity;
        await rooms.SaveChangesAsync(ct);
        return room.ToDto();
    }

    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        var room = await GetExistingAsync(id, ct);

        if (await rooms.HasReservationsAsync(id, ct))
            throw new BusinessRuleException("Cette salle a encore des réservations.");

        rooms.Remove(room);
        await rooms.SaveChangesAsync(ct);
    }

    private async Task<Room> GetExistingAsync(int id, CancellationToken ct)
        => await rooms.GetByIdAsync(id, ct)
           ?? throw new NotFoundException($"Salle {id} introuvable.");
}
