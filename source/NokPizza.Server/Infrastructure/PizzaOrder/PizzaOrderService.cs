using Microsoft.EntityFrameworkCore;
using NokPizza.Server.Database;
using NokPizza.Server.Database.Models;
using NokPizza.Server.Infrastructure.Dto;
using NokPizza.Server.Services.PizzaOrder;
using PizzaOrderModel = NokPizza.Server.Database.Models.PizzaOrder;

namespace NokPizza.Server.Infrastructure.PizzaOrder;

public class PizzaOrderService(NokPizzaDbContext dbContext) : IPizzaOrderService
{
    public async Task<Guid> CreateAsync(DateTime endTime)
    {
        var order = new PizzaOrderModel { EndTime = endTime };
        dbContext.PizzaOrders.Add(order);
        await dbContext.SaveChangesAsync();

        return order.Id;
    }

    public async Task<PizzaOrderResponse?> GetAsync(Guid id)
    {
        var order = await dbContext
            .PizzaOrders.Include(x => x.PizzaOrderConstraints)
                .ThenInclude(x => x.DietaryConstraint)
            .FirstOrDefaultAsync(x => x.Id == id);

        return order is null ? null : MapToResponse(order);
    }

    public async Task<PizzaOrderResponse?> AttendAsync(Guid orderId, IEnumerable<int> constraintIds)
    {
        var order = await dbContext
            .PizzaOrders.Include(x => x.PizzaOrderConstraints)
                .ThenInclude(x => x.DietaryConstraint)
            .FirstOrDefaultAsync(x => x.Id == orderId);

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

        await dbContext.SaveChangesAsync();
        return MapToResponse(order);
    }

    public async Task<IEnumerable<DietaryConstraint>> GetConstraintsAsync() =>
        await dbContext.DietaryConstraints.ToListAsync();

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
