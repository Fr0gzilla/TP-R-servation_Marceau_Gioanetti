using Booking.Dto;
using Booking.Services;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Api.Controllers;

[ApiController]
[Route("api/rooms")]
public class RoomsController(IRoomService service) : ControllerBase
{
    [HttpGet]
    public Task<IReadOnlyList<RoomResponse>> GetAll(CancellationToken ct)
        => service.GetAllAsync(ct);

    [HttpGet("{id:int}")]
    public Task<RoomResponse> GetById(int id, CancellationToken ct)
        => service.GetAsync(id, ct);

    // GET /api/rooms/available?startsAt=2026-12-01T14:00&endsAt=2026-12-01T16:00
    [HttpGet("available")]
    public Task<IReadOnlyList<RoomResponse>> GetAvailable(
        [FromQuery] DateTime startsAt, [FromQuery] DateTime endsAt, CancellationToken ct)
        => service.GetAvailableAsync(startsAt, endsAt, ct);

    // GET /api/rooms/1/reservations : le planning de la salle
    [HttpGet("{id:int}/reservations")]
    public Task<IReadOnlyList<ReservationResponse>> GetReservations(int id, CancellationToken ct)
        => service.GetReservationsAsync(id, ct);

    [HttpPost]
    public async Task<ActionResult<RoomResponse>> Create(RoomRequest request, CancellationToken ct)
    {
        var room = await service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = room.Id }, room);
    }

    [HttpPut("{id:int}")]
    public Task<RoomResponse> Update(int id, RoomRequest request, CancellationToken ct)
        => service.UpdateAsync(id, request, ct);

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await service.DeleteAsync(id, ct);
        return NoContent();
    }
}
