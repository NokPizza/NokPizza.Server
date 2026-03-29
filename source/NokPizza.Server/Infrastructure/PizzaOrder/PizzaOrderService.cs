using Microsoft.EntityFrameworkCore;
using NokPizza.Server.Database;
using NokPizza.Server.Database.Models;
using NokPizza.Server.Infrastructure.Dto.PizzaOrder;
using NokPizza.Server.Services.Password;
using NokPizza.Server.Services.PizzaOrder;

namespace NokPizza.Server.Infrastructure.PizzaOrder;

public partial class PizzaOrderService(
    NokPizzaDbContext dbContext,
    TimeProvider timeProvider,
    IPasswordService passwordService
) : IPizzaOrderService
{
    private readonly NokPizzaDbContext _dbContext = dbContext;
    private readonly TimeProvider _timeProvider = timeProvider;
    private readonly IPasswordService _passwordService = passwordService;

    public async Task<Guid> CreateAsync(
        CreatePizzaOrderRequestDto request,
        CancellationToken cancellationToken
    )
    {
        var order = new PizzaOrderModel
        {
            CreatorEmail = TrimOptional(request.CreatorEmail),
            NormalizedCreatorEmail = NormalizeOptional(request.CreatorEmail),
            AdminPasswordHash = _passwordService.HashPassword(request.AdminPassword),
            ParticipantPasswordHash = string.IsNullOrWhiteSpace(request.ParticipantPassword)
                ? null
                : _passwordService.HashPassword(request.ParticipantPassword),
            EndTime = request.EndTime,
            RsvpDeadline = request.RsvpDeadline,
        };

        _dbContext.PizzaOrders.Add(order);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return order.Id;
    }

    public async Task<PizzaOrderResponseDto?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var order = await _dbContext
            .PizzaOrders.Include(x => x.PizzaOrderParticipants)
                .ThenInclude(x => x.PizzaOrderParticipantConstraints)
                    .ThenInclude(x => x.DietaryConstraint)
            .FirstOrDefaultAsync(
                x => x.Id == id && x.EndTime >= _timeProvider.GetUtcNow().UtcDateTime,
                cancellationToken
            );

        return order is null ? null : MapToResponse(order);
    }

    public async Task<IEnumerable<PizzaOrderLookupResponseDto>> FindByEmailAsync(
        string email,
        CancellationToken cancellationToken
    )
    {
        var normalizedEmail = NormalizeRequired(email);
        var now = _timeProvider.GetUtcNow().UtcDateTime;

        return await _dbContext
            .PizzaOrders.Where(x =>
                x.EndTime >= now
                && (
                    x.NormalizedCreatorEmail == normalizedEmail
                    || x.PizzaOrderParticipants.Any(y => y.NormalizedEmail == normalizedEmail)
                )
            )
            .OrderBy(x => x.EndTime)
            .Select(x => new PizzaOrderLookupResponseDto(
                x.Id,
                x.EndTime,
                x.RsvpDeadline,
                x.PizzaOrderParticipants.Count,
                !string.IsNullOrWhiteSpace(x.ParticipantPasswordHash),
                x.NormalizedCreatorEmail == normalizedEmail,
                x.PizzaOrderParticipants.Any(y => y.NormalizedEmail == normalizedEmail)
            ))
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<DietaryConstraintModel>> GetConstraintsAsync(
        CancellationToken cancellationToken
    ) => await _dbContext.DietaryConstraints.ToListAsync(cancellationToken);

    public async Task DeleteExpiredAsync(CancellationToken cancellationToken = default) =>
        await _dbContext
            .PizzaOrders.Where(o => o.EndTime < _timeProvider.GetUtcNow().UtcDateTime)
            .ExecuteDeleteAsync(cancellationToken);
}
