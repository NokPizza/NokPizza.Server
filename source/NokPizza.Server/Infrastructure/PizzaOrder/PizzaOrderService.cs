using Microsoft.EntityFrameworkCore;
using NokPizza.Server.Database;
using NokPizza.Server.Database.Models;
using NokPizza.Server.Infrastructure.Dto;
using NokPizza.Server.Services.PizzaOrder;

namespace NokPizza.Server.Infrastructure.PizzaOrder;

public class PizzaOrderService(NokPizzaDbContext dbContext, TimeProvider timeProvider)
    : IPizzaOrderService
{
    public async Task<Guid> CreateAsync(DateTime endTime, CancellationToken cancellationToken)
    {
        var order = new PizzaOrderModel { EndTime = endTime };
        dbContext.PizzaOrders.Add(order);
        await dbContext.SaveChangesAsync(cancellationToken);

        return order.Id;
    }

    public async Task<PizzaOrderResponseDto?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var order = await dbContext
            .PizzaOrders.Include(x => x.PizzaOrderConstraints)
                .ThenInclude(x => x.DietaryConstraint)
            .FirstOrDefaultAsync(
                x => x.Id == id && x.EndTime >= timeProvider.GetUtcNow().UtcDateTime,
                cancellationToken
            );

        return order is null ? null : MapToResponse(order);
    }

    public async Task<PizzaOrderResponseDto?> AttendAsync(
        Guid orderId,
        IEnumerable<int> constraintIds,
        CancellationToken cancellationToken
    )
    {
        var order = await GetActiveOrderAsync(orderId, cancellationToken);
        if (order is null)
        {
            return null;
        }

        var requestedConstraintIds = constraintIds.ToArray();
        var constraintsById = await GetConstraintsByIdAsync(requestedConstraintIds, cancellationToken);

        ApplyAttendance(order, requestedConstraintIds, constraintsById);

        await dbContext.SaveChangesAsync(cancellationToken);
        return MapToResponse(order);
    }

    public async Task<IEnumerable<DietaryConstraintModel>> GetConstraintsAsync(
        CancellationToken cancellationToken
    ) => await dbContext.DietaryConstraints.ToListAsync(cancellationToken);

    public async Task DeleteExpiredAsync(CancellationToken cancellationToken = default) =>
        await dbContext
            .PizzaOrders.Where(o => o.EndTime < timeProvider.GetUtcNow().UtcDateTime)
            .ExecuteDeleteAsync(cancellationToken);

    private async Task<PizzaOrderModel?> GetActiveOrderAsync(
        Guid orderId,
        CancellationToken cancellationToken
    )
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;

        return await dbContext
            .PizzaOrders.Include(x => x.PizzaOrderConstraints)
                .ThenInclude(x => x.DietaryConstraint)
            .FirstOrDefaultAsync(x => x.Id == orderId && x.EndTime >= now, cancellationToken);
    }

    private async Task<Dictionary<int, DietaryConstraintModel>> GetConstraintsByIdAsync(
        int[] constraintIds,
        CancellationToken cancellationToken
    ) =>
        await dbContext
            .DietaryConstraints.Where(x => constraintIds.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id, cancellationToken);

    private static void ApplyAttendance(
        PizzaOrderModel order,
        IEnumerable<int> constraintIds,
        IReadOnlyDictionary<int, DietaryConstraintModel> constraintsById
    )
    {
        order.NumberOfPeople++;

        foreach (var constraintId in constraintIds)
        {
            var existing = order.PizzaOrderConstraints.FirstOrDefault(x =>
                x.ConstraintId == constraintId
            );

            if (existing is not null)
            {
                existing.Count++;
                continue;
            }

            if (!constraintsById.TryGetValue(constraintId, out var dietaryConstraint))
            {
                continue;
            }

            order.PizzaOrderConstraints.Add(
                new PizzaOrderConstraintsModel
                {
                    PizzaOrderId = order.Id,
                    ConstraintId = constraintId,
                    DietaryConstraint = dietaryConstraint,
                    Count = 1,
                }
            );
        }
    }

    private static PizzaOrderResponseDto MapToResponse(PizzaOrderModel order) =>
        new(
            order.Id,
            order.EndTime,
            order.NumberOfPeople,
            order.PizzaOrderConstraints.Select(x => new ConstraintResponseDto(
                x.DietaryConstraint.Name,
                x.Count
            ))
        );
}
