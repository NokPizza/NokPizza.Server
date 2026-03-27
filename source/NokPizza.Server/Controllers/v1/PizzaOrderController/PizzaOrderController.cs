using Microsoft.AspNetCore.Mvc;
using NokPizza.Server.Infrastructure.Dto;
using NokPizza.Server.Services.PizzaOrder;

namespace NokPizza.Server.Controllers.v1.PizzaOrderController;

[ApiController]
[Route("api/v1/[controller]")]
public class PizzaOrderController(IPizzaOrderService service, ILogger<PizzaOrderController> logger)
    : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Guid>> Create(
        [FromBody] CreatePizzaOrderRequest request,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var id = await service.CreateAsync(request.EndTime, cancellationToken);
            logger.LogInformation("Created pizza order with ID {Id}", id);

            return CreatedAtAction(nameof(Create), new { id }, id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create pizza order");
            return StatusCode(500, "Failed to create pizza order.");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PizzaOrderResponse>> Get(
        Guid id,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var order = await service.GetAsync(id, cancellationToken);
            if (order is null)
            {
                return NotFound();
            }

            return order;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to retrieve pizza order {Id}", id);
            return StatusCode(500, "Failed to retrieve pizza order.");
        }
    }

    [HttpPost("{id}/attend")]
    public async Task<ActionResult<PizzaOrderResponse>> Attend(
        Guid id,
        [FromBody] AttendRequest request,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var order = await service.AttendAsync(id, request.ConstraintIds, cancellationToken);
            if (order is null)
            {
                return NotFound();
            }

            return order;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to attend pizza order {Id}", id);
            return StatusCode(500, "Failed to attend pizza order.");
        }
    }
}
