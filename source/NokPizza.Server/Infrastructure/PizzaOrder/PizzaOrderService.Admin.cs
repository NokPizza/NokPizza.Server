using NokPizza.Server.Infrastructure.Dto.PizzaOrderAdmin;
using NokPizza.Server.Services.PizzaOrder;

namespace NokPizza.Server.Infrastructure.PizzaOrder;

public partial class PizzaOrderService
{
    public async Task<PizzaOrderAdminAccess?> GetAdminAccessAsync(
        Guid orderId,
        string password,
        CancellationToken cancellationToken
    )
    {
        var order = await GetOrderForAdminAsync(orderId, cancellationToken);
        if (order is null)
        {
            return null;
        }

        return new PizzaOrderAdminAccess(
            order,
            _passwordService.VerifyPassword(order.AdminPasswordHash, password)
        );
    }

    public PizzaOrderAdminResponseDto GetAdmin(PizzaOrderAdminAccess access) =>
        MapToAdminResponse(access.Order);

    public async Task<bool> DeleteParticipantAsync(
        PizzaOrderAdminAccess access,
        Guid participantId,
        CancellationToken cancellationToken
    )
    {
        var participant = access.Order.PizzaOrderParticipants.FirstOrDefault(x =>
            x.Id == participantId
        );
        if (participant is null)
        {
            return false;
        }

        _dbContext.PizzaOrderParticipants.Remove(participant);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
