namespace NokPizza.Server.Infrastructure.Dto.PizzaOrder;

/// <summary>
/// DTO representing the details of a pizza order for lookup purposes, including the end time, RSVP deadline, number of participants, and whether participant password is required. It also indicates if the requester is the creator or a participant of the order.
/// </summary>
/// <param name="Id">The ID of the pizza order.</param>
/// <param name="EndTime">The end time of the pizza order.</param>
/// <param name="RsvpDeadline">The RSVP deadline for the pizza order.</param>
/// <param name="NumberOfPeople">The number of participants in the pizza order.</param>
/// <param name="RequiresParticipantPassword">Indicates if a password is required for participant access.</param>
/// <param name="IsCreator">Indicates if the requester is the creator of the pizza order.</param>
/// <param name="IsParticipant">Indicates if the requester is a participant in the pizza order.</param>
public record PizzaOrderLookupResponseDto(
    Guid Id,
    DateTime EndTime,
    DateTime RsvpDeadline,
    int NumberOfPeople,
    bool RequiresParticipantPassword,
    bool IsCreator,
    bool IsParticipant
);
