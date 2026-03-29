using NokPizza.Server.Database.Models;
using NokPizza.Server.Infrastructure.Dto.PizzaOrder;
using NokPizza.Server.Infrastructure.Dto.PizzaOrderParticipant;
using NokPizza.Server.Services.PizzaOrder;

namespace NokPizza.Server.Infrastructure.PizzaOrder;

public partial class PizzaOrderService
{
    public async Task<PizzaOrderParticipantAccess?> GetParticipantAccessAsync(
        Guid orderId,
        string? password,
        CancellationToken cancellationToken
    )
    {
        var order = await GetActiveOrderAsync(orderId, cancellationToken);
        if (order is null)
        {
            return null;
        }

        return new PizzaOrderParticipantAccess(
            order,
            ValidateParticipantPassword(order, password),
            IsRsvpOpen(order)
        );
    }

    public async Task<PizzaOrderResponseDto> SaveParticipantAsync(
        PizzaOrderParticipantAccess access,
        ParticipantAttendRequestDto request,
        CancellationToken cancellationToken
    )
    {
        var order = access.Order;
        var email = TrimRequired(request.Email);
        var normalizedEmail = NormalizeRequired(request.Email);
        var requestedConstraintIds = request.ConstraintIds.Distinct().ToArray();
        var constraintsById = await GetConstraintsByIdAsync(
            requestedConstraintIds,
            cancellationToken
        );

        var participant = order.PizzaOrderParticipants.FirstOrDefault(x =>
            x.NormalizedEmail == normalizedEmail
        );

        if (participant is null)
        {
            participant = new PizzaOrderParticipantModel
            {
                PizzaOrderId = order.Id,
                PizzaOrder = order,
                Email = email,
                NormalizedEmail = normalizedEmail,
                JoinedAt = _timeProvider.GetUtcNow().UtcDateTime,
            };
            _dbContext.PizzaOrderParticipants.Add(participant);
        }
        else
        {
            participant.Email = email;
        }

        ReplaceParticipantConstraints(participant, requestedConstraintIds, constraintsById);

        await _dbContext.SaveChangesAsync(cancellationToken);
        return MapToResponse(order);
    }

    public async Task<bool> RemoveParticipantAsync(
        PizzaOrderParticipantAccess access,
        string email,
        CancellationToken cancellationToken
    )
    {
        var participant = access.Order.PizzaOrderParticipants.FirstOrDefault(x =>
            x.NormalizedEmail == NormalizeRequired(email)
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
