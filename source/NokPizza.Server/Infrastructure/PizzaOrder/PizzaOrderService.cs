using Microsoft.EntityFrameworkCore;
using NokPizza.Server.Database;
using NokPizza.Server.Database.Models;
using NokPizza.Server.Infrastructure.Dto;
using NokPizza.Server.Services.PizzaOrder;
using PizzaOrderModel = NokPizza.Server.Database.Models.PizzaOrder;

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

    public async Task<PizzaOrderResponse?> GetAsync(Guid id, CancellationToken cancellationToken)
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

    public async Task<PizzaOrderResponse?> AttendAsync(
        Guid orderId,
        IEnumerable<int> constraintIds,
        CancellationToken cancellationToken
    )
    {
        var order = await dbContext
            .PizzaOrders.Include(x => x.PizzaOrderConstraints)
                .ThenInclude(x => x.DietaryConstraint)
            .FirstOrDefaultAsync(x => x.Id == orderId, cancellationToken);

        if (order is null)
            return null;

        order.NumberOfPeople++;

        foreach (var constraintId in constraintIds)
        {
            var existing = order.PizzaOrderConstraints.FirstOrDefault(x =>
                x.ConstraintId == constraintId
            );

            if (existing is not null)
                existing.Count++;
            else
                order.PizzaOrderConstraints.Add(
                    new PizzaOrderConstraints
                    {
                        PizzaOrderId = orderId,
                        ConstraintId = constraintId,
                        Count = 1,
                    }
                );
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return MapToResponse(order);
    }

    public async Task<IEnumerable<DietaryConstraint>> GetConstraintsAsync(
        CancellationToken cancellationToken
    ) => await dbContext.DietaryConstraints.ToListAsync(cancellationToken);

    public async Task DeleteExpiredAsync(CancellationToken cancellationToken = default) =>
        await dbContext
            .PizzaOrders.Where(o => o.EndTime < timeProvider.GetUtcNow().UtcDateTime)
            .ExecuteDeleteAsync(cancellationToken);

    private static PizzaOrderResponse MapToResponse(PizzaOrderModel order) =>
        new(
            order.Id,
            order.EndTime,
            order.NumberOfPeople,
            order.PizzaOrderConstraints.Select(x => new ConstraintResponse(
                x.DietaryConstraint.Name,
                x.Count
            ))
        );
}
