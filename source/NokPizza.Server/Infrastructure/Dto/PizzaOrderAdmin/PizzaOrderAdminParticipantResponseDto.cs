namespace NokPizza.Server.Infrastructure.Dto.PizzaOrderAdmin;

/// <summary>
/// DTO representing the details of a participant in a pizza order for admin purposes.
/// </summary>
/// <param name="Id">The ID of the participant.</param>
/// <param name="Email">The email of the participant.</param>
/// <param name="JoinedAt">The date and time the participant joined the pizza order.</param>
/// <param name="Constraints">The list of dietary constraints for the participant.</param>
public record PizzaOrderAdminParticipantResponseDto(
    Guid Id,
    string Email,
    DateTime JoinedAt,
    IEnumerable<string> Constraints
);
