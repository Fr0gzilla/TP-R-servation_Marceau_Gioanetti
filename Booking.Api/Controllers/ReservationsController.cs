using Booking.Dto;
using Booking.Services;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Api.Controllers;

[ApiController]
[Route("api/reservations")]
public class ReservationsController(IReservationService service) : ControllerBase
{
    // GET api/reservations, renvoie la liste de toutes les réservations
    [HttpGet]
    public Task<IReadOnlyList<ReservationResponse>> GetAll(CancellationToken ct)
        => service.GetAllAsync(ct);

    //Get api/reservation/
    [HttpGet("{id:int}")]
    public Task<ReservationResponse> GetById(int id, CancellationToken ct)
        => service.GetAsync(id, ct);

    //
    [HttpPost]
    public async Task<ActionResult<ReservationResponse>> Create(ReservationRequest request, CancellationToken ct)
    {
        var reservation = await service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = reservation.Id }, reservation);
    }

    //
    [HttpPut("{id:int}")]
    public Task<ReservationResponse> Update(int id, ReservationRequest request, CancellationToken ct)
        => service.UpdateAsync(id, request, ct);

    //
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Cancel(int id, CancellationToken ct)
    {
        await service.CancelAsync(id, ct);
        return NoContent();
    }
}