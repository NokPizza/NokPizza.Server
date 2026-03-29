namespace NokPizza.Server.Infrastructure.Dto.PizzaOrder;

/// <summary>
/// DTO representing the details of a pizza order, including the end time, RSVP deadline, number of participants, whether participant password is required, and the list of dietary constraints with their respective counts.
/// </summary>
/// <param name="Id">The ID of the pizza order.</param>
/// <param name="EndTime">The end time of the pizza order.</param>
/// <param name="RsvpDeadline">The RSVP deadline for the pizza order.</param>
/// <param name="NumberOfPeople">The number of participants in the pizza order.</param>
/// <param name="ParticipantPasswordRequired">Indicates if a password is required for participant access.</param>
/// <param name="Constraints">The list of dietary constraints with their respective counts.</param>
public record PizzaOrderResponseDto(
    Guid Id,
    DateTime EndTime,
    DateTime RsvpDeadline,
    int NumberOfPeople,
    bool ParticipantPasswordRequired,
    IEnumerable<ConstraintResponseDto> Constraints
);
