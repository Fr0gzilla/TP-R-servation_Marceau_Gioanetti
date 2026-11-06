using Booking.Dto;
using Booking.Services;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Api.Controllers;


[ApiController]
[Route("api/users")]
public class UsersController(IUserService service) : ControllerBase
{
    [HttpGet]
    public Task<IReadOnlyList<UserResponse>> GetAll(CancellationToken ct)
        => service.GetAllAsync(ct);

    [HttpGet("{id:int}")]
    public Task<UserResponse> GetById(int id, CancellationToken ct)
        => service.GetAsync(id, ct);

    [HttpPost]
    public async Task<ActionResult<UserResponse>> Create(UserRequest request, CancellationToken ct)
    {
        var user = await service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    [HttpPut("{id:int}")]
    public Task<UserResponse> Update(int id, UserRequest request, CancellationToken ct)
        => service.UpdateAsync(id, request, ct);

    //
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await service.DeleteAsync(id, ct);
        return NoContent();
    }
}