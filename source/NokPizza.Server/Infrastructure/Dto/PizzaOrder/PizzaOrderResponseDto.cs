namespace NokPizza.Server.Infrastructure.Dto.PizzaOrder;

public record PizzaOrderResponseDto(
    Guid Id,
    DateTime EndTime,
    DateTime RsvpDeadline,
    int NumberOfPeople,
    bool ParticipantPasswordRequired,
    IEnumerable<ConstraintResponseDto> Constraints
);
