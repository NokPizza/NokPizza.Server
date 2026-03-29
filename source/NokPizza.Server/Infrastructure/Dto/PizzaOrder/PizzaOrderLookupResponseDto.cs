namespace NokPizza.Server.Infrastructure.Dto.PizzaOrder;

public record PizzaOrderLookupResponseDto(
    Guid Id,
    DateTime EndTime,
    DateTime RsvpDeadline,
    int NumberOfPeople,
    bool RequiresParticipantPassword,
    bool IsCreator,
    bool IsParticipant
);
