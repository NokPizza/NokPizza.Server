using Microsoft.EntityFrameworkCore;
using NokPizza.Server.Database;
using NokPizza.Server.Database.Models;
using NokPizza.Server.Infrastructure.Dto.PizzaOrder;
using NokPizza.Server.Infrastructure.Password;
using NokPizza.Server.Infrastructure.PizzaOrder;
using NokPizza.Server.Services.Password;
using NokPizza.Server.Services.PizzaOrder;
using NSubstitute;
using Shouldly;

namespace NokPizza.Server.Tests.Infrastructure.PizzaOrder;

public abstract class PizzaOrderServiceTestBase
{
    protected readonly NokPizzaDbContext DbContext;
    protected readonly TimeProvider TimeProvider;
    protected readonly IPasswordService PasswordService;
    protected readonly PizzaOrderService Service;

    protected PizzaOrderServiceTestBase()
    {
        var options = new DbContextOptionsBuilder<NokPizzaDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        DbContext = new NokPizzaDbContext(options);
        TimeProvider = Substitute.For<TimeProvider>();
        PasswordService = new PasswordService();
        SetUtcNow(new DateTime(2026, 03, 27, 12, 00, 00, DateTimeKind.Utc));

        Service = new PizzaOrderService(DbContext, TimeProvider, PasswordService);
    }

    protected void SetUtcNow(DateTime utcNow) =>
        TimeProvider.GetUtcNow().Returns(new DateTimeOffset(utcNow));

    protected async Task<Guid> CreateOrderAsync(
        string? creatorEmail = null,
        string adminPassword = "admin-password",
        string? participantPassword = null,
        DateTime? endTime = null,
        DateTime? rsvpDeadline = null
    )
    {
        var id = await Service.CreateAsync(
            new CreatePizzaOrderRequestDto(
                creatorEmail,
                adminPassword,
                participantPassword,
                endTime ?? new DateTime(2026, 03, 27, 18, 00, 00, DateTimeKind.Utc),
                rsvpDeadline ?? new DateTime(2026, 03, 27, 16, 00, 00, DateTimeKind.Utc)
            ),
            CancellationToken.None
        );

        DbContext.ChangeTracker.Clear();
        return id;
    }

    protected async Task SeedConstraintsAsync(params DietaryConstraintModel[] constraints)
    {
        DbContext.DietaryConstraints.AddRange(constraints);
        await DbContext.SaveChangesAsync();
        DbContext.ChangeTracker.Clear();
    }

    protected void ClearTracking() => DbContext.ChangeTracker.Clear();

    protected async Task<PizzaOrderParticipantAccess> GetParticipantAccessAsync(
        Guid orderId,
        string? password
    )
    {
        var access = await Service.GetParticipantAccessAsync(
            orderId,
            password,
            CancellationToken.None
        );
        access.ShouldNotBeNull();

        return access;
    }

    protected async Task<PizzaOrderAdminAccess> GetAdminAccessAsync(Guid orderId, string password)
    {
        var access = await Service.GetAdminAccessAsync(orderId, password, CancellationToken.None);
        access.ShouldNotBeNull();

        return access;
    }
}
