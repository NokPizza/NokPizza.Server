using Microsoft.AspNetCore.Mvc;
using NokPizza.Server.Infrastructure.Dto.PizzaOrderAdmin;
using NokPizza.Server.Services.PizzaOrder;

namespace NokPizza.Server.Controllers.v1.PizzaOrderController;

[ApiController]
[Route("api/v1/pizzaorder")]
public class PizzaOrderAdminController(IPizzaOrderService service) : ControllerBase
{
    /// <summary>
    /// Gets the admin details of a pizza order, including the list of participants and their dietary constraints.
    /// </summary>
    /// <param name="id">The ID of the pizza order.</param>
    /// <param name="password">The admin password.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The admin details of the pizza order.</returns>
    [HttpPost("{id}/admin")]
    public async Task<ActionResult<PizzaOrderAdminResponseDto>> GetAdmin(
        Guid id,
        [FromBody] string password,
        CancellationToken cancellationToken = default
    )
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            return BadRequest("Password is required.");
        }

        var access = await service.GetAdminAccessAsync(id, password, cancellationToken);
        if (access is null)
        {
            return NotFound("Pizza order was not found.");
        }

        if (!access.IsPasswordValid)
        {
            return Unauthorized("The provided password is invalid.");
        }

        return service.GetAdmin(access);
    }

    /// <summary>
    /// Deletes a participant from a pizza order. Only the admin can delete participants.
    /// </summary>
    /// <param name="id">The ID of the pizza order.</param>
    /// <param name="participantId">The ID of the participant to delete.</param>
    /// <param name="password">The admin password.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns></returns>
    [HttpDelete("{id}/participants/{participantId}")]
    public async Task<ActionResult> DeleteParticipant(
        Guid id,
        Guid participantId,
        [FromBody] string password,
        CancellationToken cancellationToken = default
    )
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            return BadRequest("Password is required.");
        }

        var access = await service.GetAdminAccessAsync(id, password, cancellationToken);
        if (access is null)
        {
            return NotFound("Pizza order was not found.");
        }

        if (!access.IsPasswordValid)
        {
            return Unauthorized("The provided password is invalid.");
        }

        var deleted = await service.DeleteParticipantAsync(
            access,
            participantId,
            cancellationToken
        );
        if (!deleted)
        {
            return NotFound("Participant was not found.");
        }

        return NoContent();
    }
}
