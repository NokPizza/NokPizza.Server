using Microsoft.AspNetCore.Mvc;
using NokPizza.Server.Database.Models;
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
    public async Task<ActionResult<IEnumerable<DietaryConstraint>>> GetAll()
    {
        try
        {
            return Ok(await service.GetConstraintsAsync());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to retrieve dietary constraints.");
            return StatusCode(500, "Failed to retrieve dietary constraints.");
        }
    }
}
