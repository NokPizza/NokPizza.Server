using System.ComponentModel.DataAnnotations;

namespace NokPizza.Server.Infrastructure.Dto.PizzaOrder;

/// <summary>
/// Request payload for searching pizza orders by email.
/// </summary>
public sealed record SearchPizzaOrdersRequestDto
{
    /// <summary>
    /// The email address to search for.
    /// </summary>
    [Required]
    public string Email { get; init; } = string.Empty;
}
