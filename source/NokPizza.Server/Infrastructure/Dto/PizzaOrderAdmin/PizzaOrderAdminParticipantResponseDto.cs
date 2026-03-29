namespace NokPizza.Server.Infrastructure.Dto.PizzaOrderAdmin;

public record PizzaOrderAdminParticipantResponseDto(
    Guid Id,
    string Email,
    DateTime JoinedAt,
    IEnumerable<string> Constraints
);
