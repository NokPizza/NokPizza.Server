using Microsoft.AspNetCore.Mvc;
using NokPizza.Server.Infrastructure.Dto.PizzaOrder;
using NokPizza.Server.Infrastructure.Dto.PizzaOrderParticipant;
using NokPizza.Server.Services.PizzaOrder;

namespace NokPizza.Server.Controllers.v1.PizzaOrderController;

[ApiController]
[Route("api/v1/pizzaorder")]
public class PizzaOrderParticipantController(IPizzaOrderService service) : ControllerBase
{
    /// <summary>
    /// Saves a participant's attendance for a pizza order.
    /// </summary>
    /// <param name="id">The ID of the pizza order.</param>
    /// <param name="request">The request containing the participant details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated pizza order details.</returns>
    [HttpPost("{id}/participants")]
    public async Task<ActionResult<PizzaOrderResponseDto>> SaveParticipant(
        Guid id,
        [FromBody] ParticipantAttendRequestDto request,
        CancellationToken cancellationToken = default
    )
    {
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return BadRequest("Email is required.");
        }

        var access = await service.GetParticipantAccessAsync(
            id,
            request.Password,
            cancellationToken
        );
        if (access is null)
        {
            return NotFound("Pizza order was not found.");
        }

        if (!access.IsRsvpOpen)
        {
            return Conflict("RSVP deadline has passed.");
        }

        if (!access.IsPasswordValid)
        {
            return Unauthorized("The provided password is invalid.");
        }

        return await service.SaveParticipantAsync(access, request, cancellationToken);
    }

    /// <summary>
    /// Removes a participant from a pizza order.
    /// </summary>
    /// <param name="id">The ID of the pizza order.</param>
    /// <param name="request">The request containing the participant details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns></returns>
    [HttpPost("{id}/participants/leave")]
    public async Task<ActionResult> LeaveParticipant(
        Guid id,
        [FromBody] LeavePizzaOrderParticipantRequestDto request,
        CancellationToken cancellationToken = default
    )
    {
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return BadRequest("Email is required.");
        }

        var access = await service.GetParticipantAccessAsync(
            id,
            request.Password,
            cancellationToken
        );
        if (access is null)
        {
            return NotFound("Pizza order was not found.");
        }

        if (!access.IsRsvpOpen)
        {
            return Conflict("RSVP deadline has passed.");
        }

        if (!access.IsPasswordValid)
        {
            return Unauthorized("The provided password is invalid.");
        }

        var removed = await service.RemoveParticipantAsync(
            access,
            request.Email,
            cancellationToken
        );
        if (!removed)
        {
            return NotFound("Participant was not found.");
        }

        return NoContent();
    }
}
