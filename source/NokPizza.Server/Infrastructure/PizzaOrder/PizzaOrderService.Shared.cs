using Microsoft.EntityFrameworkCore;
using NokPizza.Server.Database.Models;
using NokPizza.Server.Infrastructure.Dto.PizzaOrder;
using NokPizza.Server.Infrastructure.Dto.PizzaOrderAdmin;

namespace NokPizza.Server.Infrastructure.PizzaOrder;

public partial class PizzaOrderService
{
    private async Task<PizzaOrderModel?> GetActiveOrderAsync(
        Guid orderId,
        CancellationToken cancellationToken
    )
    {
        var now = _timeProvider.GetUtcNow().UtcDateTime;

        return await _dbContext
            .PizzaOrders.Include(x => x.PizzaOrderParticipants)
                .ThenInclude(x => x.PizzaOrderParticipantConstraints)
                    .ThenInclude(x => x.DietaryConstraint)
            .FirstOrDefaultAsync(x => x.Id == orderId && x.EndTime >= now, cancellationToken);
    }

    private async Task<PizzaOrderModel?> GetOrderForAdminAsync(
        Guid orderId,
        CancellationToken cancellationToken
    ) =>
        await _dbContext
            .PizzaOrders.Include(x => x.PizzaOrderParticipants)
                .ThenInclude(x => x.PizzaOrderParticipantConstraints)
                    .ThenInclude(x => x.DietaryConstraint)
            .FirstOrDefaultAsync(x => x.Id == orderId, cancellationToken);

    private async Task<Dictionary<int, DietaryConstraintModel>> GetConstraintsByIdAsync(
        int[] constraintIds,
        CancellationToken cancellationToken
    ) =>
        await _dbContext
            .DietaryConstraints.Where(x => constraintIds.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id, cancellationToken);

    private void ReplaceParticipantConstraints(
        PizzaOrderParticipantModel participant,
        IEnumerable<int> constraintIds,
        Dictionary<int, DietaryConstraintModel> constraintsById
    )
    {
        if (participant.PizzaOrderParticipantConstraints.Count > 0)
        {
            _dbContext.PizzaOrderParticipantConstraints.RemoveRange(
                participant.PizzaOrderParticipantConstraints
            );
            participant.PizzaOrderParticipantConstraints.Clear();
        }

        foreach (var constraintId in constraintIds)
        {
            if (!constraintsById.TryGetValue(constraintId, out var dietaryConstraint))
            {
                continue;
            }

            _dbContext.PizzaOrderParticipantConstraints.Add(
                new PizzaOrderParticipantConstraintModel
                {
                    Participant = participant,
                    ConstraintId = constraintId,
                    DietaryConstraint = dietaryConstraint,
                }
            );
        }
    }

    private bool ValidateParticipantPassword(PizzaOrderModel order, string? password) =>
        string.IsNullOrWhiteSpace(order.ParticipantPasswordHash)
        || (
            !string.IsNullOrWhiteSpace(password)
            && _passwordService.VerifyPassword(order.ParticipantPasswordHash, password)
        );

    private bool IsRsvpOpen(PizzaOrderModel order) =>
        order.RsvpDeadline >= _timeProvider.GetUtcNow().UtcDateTime;

    private static PizzaOrderResponseDto MapToResponse(PizzaOrderModel order) =>
        new(
            order.Id,
            order.EndTime,
            order.RsvpDeadline,
            order.PizzaOrderParticipants.Count,
            !string.IsNullOrWhiteSpace(order.ParticipantPasswordHash),
            order
                .PizzaOrderParticipants.SelectMany(x => x.PizzaOrderParticipantConstraints)
                .GroupBy(x => x.DietaryConstraint.Name)
                .Select(x => new ConstraintResponseDto(x.Key, x.Count()))
                .OrderBy(x => x.Name)
        );

    private static PizzaOrderAdminResponseDto MapToAdminResponse(PizzaOrderModel order) =>
        new(
            order.Id,
            order.EndTime,
            order.RsvpDeadline,
            order.CreatorEmail,
            order.PizzaOrderParticipants.Count,
            !string.IsNullOrWhiteSpace(order.ParticipantPasswordHash),
            order
                .PizzaOrderParticipants.SelectMany(x => x.PizzaOrderParticipantConstraints)
                .GroupBy(x => x.DietaryConstraint.Name)
                .Select(x => new ConstraintResponseDto(x.Key, x.Count()))
                .OrderBy(x => x.Name),
            order
                .PizzaOrderParticipants.OrderBy(x => x.JoinedAt)
                .Select(x => new PizzaOrderAdminParticipantResponseDto(
                    x.Id,
                    x.Email,
                    x.JoinedAt,
                    x.PizzaOrderParticipantConstraints.Select(y => y.DietaryConstraint.Name)
                        .OrderBy(y => y)
                ))
        );

    private static string NormalizeRequired(string email) => email.Trim().ToLowerInvariant();

    private static string? NormalizeOptional(string? email) =>
        string.IsNullOrWhiteSpace(email) ? null : NormalizeRequired(email);

    private static string TrimRequired(string value) => value.Trim();

    private static string? TrimOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
