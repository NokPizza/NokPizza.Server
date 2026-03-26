using Microsoft.AspNetCore.Mvc;
using NokPizza.Server.Infrastructure.Dto;
using NokPizza.Server.Services.PizzaOrder;

namespace NokPizza.Server.Controllers.v1.PizzaOrderController;

[ApiController]
[Route("api/v1/[controller]")]
public partial class PizzaOrderController(
    IPizzaOrderService service,
    ILogger<PizzaOrderController> logger
) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] CreatePizzaOrderRequest request)
    {
        try
        {
            var id = await service.CreateAsync(request.EndTime);
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
    public async Task<ActionResult<PizzaOrderResponse>> Get(Guid id)
    {
        try
        {
            var order = await service.GetAsync(id);
            if (order is null)
                return NotFound();

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
        [FromBody] AttendRequest request
    )
    {
        try
        {
            var order = await service.AttendAsync(id, request.ConstraintIds);
            if (order is null)
                return NotFound();

            return order;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to attend pizza order {Id}", id);
            return StatusCode(500, "Failed to attend pizza order.");
        }
    }
}
