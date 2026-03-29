using Microsoft.AspNetCore.Mvc;
using NokPizza.Server.Infrastructure.Dto.PizzaOrder;
using NokPizza.Server.Services.PizzaOrder;

namespace NokPizza.Server.Controllers.v1.PizzaOrderController;

/// <summary>
/// Controller for managing pizza orders.
/// </summary>
/// <param name="service">The pizza order service.</param>
[ApiController]
[Route("api/v1/[controller]")]
public class PizzaOrderController(IPizzaOrderService service) : ControllerBase
{
    /// <summary>
    /// Creates a new pizza order with the specified details. The admin password is required to create the order, and the RSVP deadline must be before or equal to the end time.
    /// </summary>
    /// <param name="request">The request containing the pizza order details.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The ID of the created pizza order.</returns>
    [HttpPost]
    public async Task<ActionResult<Guid>> Create(
        [FromBody] CreatePizzaOrderRequestDto request,
        CancellationToken cancellationToken = default
    )
    {
        if (string.IsNullOrWhiteSpace(request.AdminPassword))
        {
            return BadRequest("Admin password is required.");
        }

        if (request.RsvpDeadline > request.EndTime)
        {
            return BadRequest("RSVP deadline must be before or equal to end time.");
        }

        var id = await service.CreateAsync(request, cancellationToken);

        return CreatedAtAction(nameof(Get), new { id }, id);
    }

    /// <summary>
    /// Gets the details of a pizza order by its ID.
    /// </summary>
    /// <param name="id">The ID of the pizza order.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The details of the pizza order.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<PizzaOrderResponseDto>> Get(
        Guid id,
        CancellationToken cancellationToken = default
    )
    {
        var order = await service.GetAsync(id, cancellationToken);
        if (order is null)
        {
            return NotFound();
        }

        return order;
    }

    /// <summary>
    /// Finds pizza orders by email. This will return all pizza orders where the specified email is.
    /// </summary>
    /// <param name="email">The email to search for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The list of matching pizza orders.</returns>
    [HttpPost("search")]
    public async Task<ActionResult<IEnumerable<PizzaOrderLookupResponseDto>>> Search(
        [FromBody] string email,
        CancellationToken cancellationToken = default
    )
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return BadRequest("Email is required.");
        }

        var orders = await service.FindByEmailAsync(email, cancellationToken);
        return Ok(orders);
    }
}
