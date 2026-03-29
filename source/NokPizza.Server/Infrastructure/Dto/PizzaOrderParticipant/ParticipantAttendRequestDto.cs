namespace NokPizza.Server.Infrastructure.Dto.PizzaOrderParticipant;

public record ParticipantAttendRequestDto(
    string Email,
    string? Password,
    IEnumerable<int> ConstraintIds
);
