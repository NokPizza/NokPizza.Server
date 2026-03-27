using Microsoft.AspNetCore.Mvc;
using NokPizza.Server.Infrastructure.Dto;
using NokPizza.Server.Services.PizzaOrder;

namespace NokPizza.Server.Controllers.v1.DietaryConstraintController;

[ApiController]
[Route("api/v1/[controller]")]
public class DietaryConstraintController(
    IPizzaOrderService service,
    ILogger<DietaryConstraintController> logger
) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DietaryConstraintResponseDto>>> GetAllConstraints(
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var constraints = await service.GetConstraintsAsync(cancellationToken);
            var response = constraints.Select(x => new DietaryConstraintResponseDto(x.Id, x.Name));

            return Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to retrieve dietary constraints.");
            return StatusCode(500, "Failed to retrieve dietary constraints.");
        }
    }
}
