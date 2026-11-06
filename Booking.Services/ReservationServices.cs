using Booking.DataAccess.Interfaces;
using Booking.Dto;
using Booking.Entities;
using Booking.Entities.Exceptions;

namespace Booking.Services;

public interface IReservationService
{
    Task<IReadOnlyList<ReservationResponse>> GetAllAsync(CancellationToken ct);
    Task<ReservationResponse> GetAsync(int id, CancellationToken ct);
    Task<ReservationResponse> CreateAsync(ReservationRequest request, CancellationToken ct);
    Task<ReservationResponse> UpdateAsync(int id, ReservationRequest request, CancellationToken ct);
    Task CancelAsync(int id, CancellationToken ct);
}

public class ReservationService(
    IReservationRepository reservations,
    IRoomRepository rooms,
    IUserRepository users) : IReservationService
{
    public async Task<IReadOnlyList<ReservationResponse>> GetAllAsync(CancellationToken ct)
        => (await reservations.GetAllAsync(ct)).Select(r => r.ToDto()).ToList();

    public async Task<ReservationResponse> GetAsync(int id, CancellationToken ct)
        => (await GetExistingAsync(id, ct)).ToDto();

    public async Task<ReservationResponse> CreateAsync(ReservationRequest request, CancellationToken ct)
    {
        var room = await CheckRulesAsync(request, null, ct);

        var reservation = new Reservation
        {
            UserId = request.UserId!.Value,
            Room = room,                        // la réservation récupère la salle demandée
            StartsAt = request.StartsAt!.Value,
            EndsAt = request.EndsAt!.Value,
            Attendees = request.Attendees
        };

        reservations.Add(reservation);
        await reservations.SaveChangesAsync(ct);
        return reservation.ToDto();
    }

    public async Task<ReservationResponse> UpdateAsync(int id, ReservationRequest request, CancellationToken ct)
    {
        var reservation = await GetExistingAsync(id, ct);
        var room = await CheckRulesAsync(request, id, ct);   // id : on ignore son propre créneau

        reservation.UserId = request.UserId!.Value;
        reservation.Room = room;
        reservation.StartsAt = request.StartsAt!.Value;
        reservation.EndsAt = request.EndsAt!.Value;
        reservation.Attendees = request.Attendees;

        await reservations.SaveChangesAsync(ct);
        return reservation.ToDto();
    }

    public async Task CancelAsync(int id, CancellationToken ct)
    {
        var reservation = await GetExistingAsync(id, ct);
        reservations.Remove(reservation);
        await reservations.SaveChangesAsync(ct);
    }

    // toutes les règles métier d'une réservation, utilisées au POST et au PUT
    private async Task<Room> CheckRulesAsync(ReservationRequest request, int? exceptId, CancellationToken ct)
    {
        var startsAt = request.StartsAt!.Value;
        var endsAt = request.EndsAt!.Value;

        // 1. créneau cohérent -> 400
        if (startsAt >= endsAt)
            throw new ArgumentException("La date de début doit être avant la date de fin.");

        // 2. pas dans le passé -> 400
        if (startsAt < DateTime.Now)
            throw new ArgumentException("Impossible de réserver dans le passé.");

        // 3. l'utilisateur existe -> 404
        if (!await users.ExistsAsync(request.UserId!.Value, ct))
            throw new NotFoundException($"Utilisateur {request.UserId} introuvable.");

        // 4. la salle existe -> 404
        var room = await rooms.GetByIdAsync(request.RoomId!.Value, ct)
                   ?? throw new NotFoundException($"Salle {request.RoomId} introuvable.");

        // 5. capacité -> 409
        if (request.Attendees > room.Capacity)
            throw new BusinessRuleException($"La salle ne peut accueillir que {room.Capacity} personnes.");

        // 6. pas de chevauchement -> 409
        if (await reservations.OverlapsAsync(room.Id, startsAt, endsAt, exceptId, ct))
            throw new BusinessRuleException("La salle est déjà réservée sur ce créneau.");

        return room;
    }

    private async Task<Reservation> GetExistingAsync(int id, CancellationToken ct)
        => await reservations.GetByIdAsync(id, ct)
           ?? throw new NotFoundException($"Réservation {id} introuvable.");
}
