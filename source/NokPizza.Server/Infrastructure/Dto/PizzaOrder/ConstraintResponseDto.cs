namespace NokPizza.Server.Infrastructure.Dto.PizzaOrder;

/// <summary>
/// DTO representing a dietary constraint and the count of participants with that constraint for a pizza order.
/// </summary>
/// <param name="Name">The name of the dietary constraint.</param>
/// <param name="Count">The count of participants with the dietary constraint.</param>
public record ConstraintResponseDto(string Name, int Count);
