using NokPizza.Server.Database.Models;
using NokPizza.Server.Infrastructure.Dto;

namespace NokPizza.Server.Services.PizzaOrder;

public interface IPizzaOrderService
{
    Task<Guid> CreateAsync(DateTime endTime, CancellationToken cancellationToken);
    Task<PizzaOrderResponseDto?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<PizzaOrderResponseDto?> AttendAsync(
        Guid orderId,
        IEnumerable<int> constraintIds,
        CancellationToken cancellationToken
    );
    Task<IEnumerable<DietaryConstraintModel>> GetConstraintsAsync(
        CancellationToken cancellationToken
    );
    Task DeleteExpiredAsync(CancellationToken cancellationToken);
}
