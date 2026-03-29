using NokPizza.Server.Database.Models;
using NokPizza.Server.Infrastructure.Dto.PizzaOrder;
using NokPizza.Server.Infrastructure.Dto.PizzaOrderAdmin;
using NokPizza.Server.Infrastructure.Dto.PizzaOrderParticipant;

namespace NokPizza.Server.Services.PizzaOrder;

public interface IPizzaOrderService
{
    Task<Guid> CreateAsync(CreatePizzaOrderRequestDto request, CancellationToken cancellationToken);

    Task<PizzaOrderResponseDto?> GetAsync(Guid id, CancellationToken cancellationToken);

    Task<IEnumerable<PizzaOrderLookupResponseDto>> FindByEmailAsync(
        string email,
        CancellationToken cancellationToken
    );

    Task<PizzaOrderParticipantAccess?> GetParticipantAccessAsync(
        Guid orderId,
        string? password,
        CancellationToken cancellationToken
    );

    Task<PizzaOrderResponseDto> SaveParticipantAsync(
        PizzaOrderParticipantAccess access,
        ParticipantAttendRequestDto request,
        CancellationToken cancellationToken
    );

    Task<bool> RemoveParticipantAsync(
        PizzaOrderParticipantAccess access,
        string email,
        CancellationToken cancellationToken
    );

    Task<PizzaOrderAdminAccess?> GetAdminAccessAsync(
        Guid orderId,
        string password,
        CancellationToken cancellationToken
    );

    PizzaOrderAdminResponseDto GetAdmin(PizzaOrderAdminAccess access);

    Task<bool> DeleteParticipantAsync(
        PizzaOrderAdminAccess access,
        Guid participantId,
        CancellationToken cancellationToken
    );

    Task<IEnumerable<DietaryConstraintModel>> GetConstraintsAsync(
        CancellationToken cancellationToken
    );

    Task DeleteExpiredAsync(CancellationToken cancellationToken);
}
