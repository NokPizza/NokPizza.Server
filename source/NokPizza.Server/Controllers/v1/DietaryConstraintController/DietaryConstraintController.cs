using Microsoft.AspNetCore.Mvc;
using NokPizza.Server.Infrastructure.Dto;
using NokPizza.Server.Services.PizzaOrder;

namespace NokPizza.Server.Controllers.v1.DietaryConstraintController;

/// <summary>
/// Controller for managing dietary constraints.
/// </summary>
/// <param name="service">The pizza order service.</param>
[ApiController]
[Route("api/v1/[controller]")]
public class DietaryConstraintController(IPizzaOrderService service) : ControllerBase
{
    /// <summary>
    /// Gets all dietary constraints available for pizza orders.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The list of dietary constraints.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DietaryConstraintResponseDto>>> GetAllConstraints(
        CancellationToken cancellationToken = default
    )
    {
        var constraints = await service.GetConstraintsAsync(cancellationToken);
        var response = constraints.Select(x => new DietaryConstraintResponseDto(x.Id, x.Name));

        return Ok(response);
    }
}
