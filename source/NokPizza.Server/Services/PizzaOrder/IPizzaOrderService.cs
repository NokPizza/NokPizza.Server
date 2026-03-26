using NokPizza.Server.Database.Models;
using NokPizza.Server.Infrastructure.Dto;

namespace NokPizza.Server.Services.PizzaOrder;

public interface IPizzaOrderService
{
    Task<Guid> CreateAsync(DateTime endTime);
    Task<PizzaOrderResponse?> GetAsync(Guid id);
    Task<PizzaOrderResponse?> AttendAsync(Guid orderId, IEnumerable<int> constraintIds);
    Task<IEnumerable<DietaryConstraint>> GetConstraintsAsync();
}
