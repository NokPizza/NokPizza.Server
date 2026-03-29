namespace NokPizza.Server.Infrastructure.Dto.PizzaOrder;

/// <summary>
/// DTO representing the request to create a new pizza order. The admin password is required to create the order, and the RSVP deadline must be before or equal to the end time.
/// </summary>
/// <param name="CreatorEmail">The email of the user creating the pizza order.</param>
/// <param name="AdminPassword">The password for admin access to the pizza order.</param>
/// <param name="ParticipantPassword">The password for participant access to the pizza order.</param>
/// <param name="EndTime">The end time of the pizza order.</param>
/// <param name="RsvpDeadline">The RSVP deadline for the pizza order.</param>
public record CreatePizzaOrderRequestDto(
    string? CreatorEmail,
    string AdminPassword,
    string? ParticipantPassword,
    DateTime EndTime,
    DateTime RsvpDeadline
);
