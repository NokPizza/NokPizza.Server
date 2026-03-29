using NokPizza.Server.Infrastructure.Dto.PizzaOrder;

namespace NokPizza.Server.Infrastructure.Dto.PizzaOrderAdmin;

/// <summary>
/// DTO representing the details of a pizza order for admin purposes.
/// </summary>
/// <param name="Id">The ID of the pizza order.</param>
/// <param name="EndTime">The end time of the pizza order.</param>
/// <param name="RsvpDeadline">The RSVP deadline for the pizza order.</param>
/// <param name="CreatorEmail">The email of the user creating the pizza order.</param>
/// <param name="NumberOfPeople">The number of participants in the pizza order.</param>
/// <param name="ParticipantPasswordRequired">Indicates if a password is required for participant access.</param>
/// <param name="Constraints">The list of dietary constraints with their respective counts.</param>
/// <param name="Participants">The list of participants in the pizza order.</param>
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
