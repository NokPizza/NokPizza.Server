namespace NokPizza.Server.Infrastructure.Dto.PizzaOrderParticipant;

/// <summary>
/// DTO representing the request for a participant to attend a pizza order.
/// </summary>
/// <param name="Email">The email of the participant.</param>
/// <param name="Password">The password for participant access to the pizza order.</param>
/// <param name="ConstraintIds">The list of dietary constraint IDs for the participant.</param>
public record ParticipantAttendRequestDto(
    string Email,
    string? Password,
    IEnumerable<int> ConstraintIds
);
