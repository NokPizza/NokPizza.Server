namespace NokPizza.Server.Infrastructure.Dto.PizzaOrder;

public record CreatePizzaOrderRequestDto(
    string? CreatorEmail,
    string AdminPassword,
    string? ParticipantPassword,
    DateTime EndTime,
    DateTime RsvpDeadline
);
