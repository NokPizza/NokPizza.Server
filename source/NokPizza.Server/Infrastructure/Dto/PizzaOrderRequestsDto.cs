namespace NokPizza.Server.Infrastructure.Dto;

public record CreatePizzaOrderRequestDto(DateTime EndTime);

public record AttendRequestDto(IEnumerable<int> ConstraintIds);
