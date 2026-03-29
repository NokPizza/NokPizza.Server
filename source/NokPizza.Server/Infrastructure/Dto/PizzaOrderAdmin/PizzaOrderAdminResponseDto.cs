using NokPizza.Server.Infrastructure.Dto.PizzaOrder;

namespace NokPizza.Server.Infrastructure.Dto.PizzaOrderAdmin;

public record PizzaOrderAdminResponseDto(
    Guid Id,
    DateTime EndTime,
    DateTime RsvpDeadline,
    string? CreatorEmail,
    int NumberOfPeople,
    bool ParticipantPasswordRequired,
    IEnumerable<ConstraintResponseDto> Constraints,
    IEnumerable<PizzaOrderAdminParticipantResponseDto> Participants
);
