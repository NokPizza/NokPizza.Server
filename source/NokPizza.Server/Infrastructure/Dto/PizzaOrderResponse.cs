namespace NokPizza.Server.Infrastructure.Dto;

public record PizzaOrderResponse(
    Guid Id,
    DateTime EndTime,
    int NumberOfPeople,
    IEnumerable<ConstraintResponse> Constraints
);

public record ConstraintResponse(string Name, int Count);
