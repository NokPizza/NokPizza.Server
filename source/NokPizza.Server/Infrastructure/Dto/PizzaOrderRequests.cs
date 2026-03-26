namespace NokPizza.Server.Infrastructure.Dto;

public record CreatePizzaOrderRequest(DateTime EndTime);

public record AttendRequest(IEnumerable<int> ConstraintIds);
