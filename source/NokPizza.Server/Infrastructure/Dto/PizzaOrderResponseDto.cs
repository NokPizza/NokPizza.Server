namespace NokPizza.Server.Infrastructure.Dto;

public record PizzaOrderResponseDto(
    Guid Id,
    DateTime EndTime,
    int NumberOfPeople,
    IEnumerable<ConstraintResponseDto> Constraints
);

public record ConstraintResponseDto(string Name, int Count);
