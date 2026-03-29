namespace NokPizza.Server.Infrastructure.Dto.PizzaOrderParticipant;

/// <summary>
/// DTO representing the request for a participant to leave a pizza order.
/// </summary>
/// <param name="Email">The email of the participant.</param>
/// <param name="Password">The password for participant access to the pizza order.</param>
public record LeavePizzaOrderParticipantRequestDto(string Email, string? Password);
