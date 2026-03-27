using Microsoft.EntityFrameworkCore;
using NokPizza.Server.Database;
using NokPizza.Server.Database.Models;
using NokPizza.Server.Infrastructure.PizzaOrder;
using NSubstitute;
using Shouldly;

namespace NokPizza.Server.Tests.Infrastructure.PizzaOrder;

public class PizzaOrderServiceTests
{
    private readonly NokPizzaDbContext _dbContext;
    private readonly TimeProvider _timeProvider;
    private readonly PizzaOrderService _service;

    public PizzaOrderServiceTests()
    {
        var dbContextId = Guid.NewGuid().ToString();
        var options = new DbContextOptionsBuilder<NokPizzaDbContext>()
            .UseInMemoryDatabase(dbContextId)
            .Options;

        _dbContext = new NokPizzaDbContext(options);
        _timeProvider = Substitute.For<TimeProvider>();
        SetUtcNow(new DateTime(2026, 03, 27, 12, 00, 00, DateTimeKind.Utc));
        _service = new PizzaOrderService(_dbContext, _timeProvider);
    }

    [Fact]
    public async Task CreateAsync_Saves_Order_And_Returns_Id()
    {
        var endTime = new DateTime(2026, 03, 27, 18, 00, 00, DateTimeKind.Utc);

        var id = await _service.CreateAsync(endTime, CancellationToken.None);

        var savedOrder = await _dbContext.PizzaOrders.SingleAsync(x => x.Id == id);
        savedOrder.EndTime.ShouldBe(endTime);
        savedOrder.NumberOfPeople.ShouldBe(0);
    }

    [Fact]
    public async Task GetAsync_Returns_Null_For_Expired_Order()
    {
        SetUtcNow(new DateTime(2026, 03, 27, 12, 00, 00, DateTimeKind.Utc));

        var order = new PizzaOrderModel
        {
            EndTime = new DateTime(2026, 03, 27, 11, 00, 00, DateTimeKind.Utc),
        };
        _dbContext.PizzaOrders.Add(order);
        await _dbContext.SaveChangesAsync();

        var result = await _service.GetAsync(order.Id, CancellationToken.None);

        result.ShouldBeNull();
    }

    [Fact]
    public async Task GetAsync_Returns_Order_With_Constraints_For_Active_Order()
    {
        SetUtcNow(new DateTime(2026, 03, 27, 12, 00, 00, DateTimeKind.Utc));

        var vegan = new DietaryConstraintModel { Id = 1, Name = "Vegan" };
        var order = new PizzaOrderModel
        {
            EndTime = new DateTime(2026, 03, 27, 13, 00, 00, DateTimeKind.Utc),
            NumberOfPeople = 2,
            PizzaOrderConstraints =
            [
                new PizzaOrderConstraintsModel
                {
                    ConstraintId = vegan.Id,
                    DietaryConstraint = vegan,
                    Count = 2,
                },
            ],
        };
        _dbContext.PizzaOrders.Add(order);
        await _dbContext.SaveChangesAsync();

        var result = await _service.GetAsync(order.Id, CancellationToken.None);

        result.ShouldNotBeNull();
        result.Id.ShouldBe(order.Id);
        result.NumberOfPeople.ShouldBe(2);

        var constraint = result.Constraints.ShouldHaveSingleItem();
        constraint.Name.ShouldBe("Vegan");
        constraint.Count.ShouldBe(2);
    }

    [Fact]
    public async Task AttendAsync_Increments_People_And_Adds_New_Constraint_Counts()
    {
        SetUtcNow(new DateTime(2026, 03, 27, 12, 00, 00, DateTimeKind.Utc));

        _dbContext.DietaryConstraints.AddRange(
            new DietaryConstraintModel { Id = 1, Name = "Vegan" },
            new DietaryConstraintModel { Id = 2, Name = "Gluten" }
        );

        var order = new PizzaOrderModel
        {
            EndTime = new DateTime(2026, 03, 27, 13, 00, 00, DateTimeKind.Utc),
            NumberOfPeople = 1,
        };
        _dbContext.PizzaOrders.Add(order);
        await _dbContext.SaveChangesAsync();

        var result = await _service.AttendAsync(order.Id, [1, 2], CancellationToken.None);

        result.ShouldNotBeNull();
        result.NumberOfPeople.ShouldBe(2);

        var constraints = result.Constraints.OrderBy(x => x.Name).ToArray();
        constraints.Length.ShouldBe(2);
        constraints[0].Name.ShouldBe("Gluten");
        constraints[0].Count.ShouldBe(1);
        constraints[1].Name.ShouldBe("Vegan");
        constraints[1].Count.ShouldBe(1);
    }

    [Fact]
    public async Task AttendAsync_Increments_Existing_Constraint_Count()
    {
        SetUtcNow(new DateTime(2026, 03, 27, 12, 00, 00, DateTimeKind.Utc));

        var vegan = new DietaryConstraintModel { Id = 1, Name = "Vegan" };
        var order = new PizzaOrderModel
        {
            EndTime = new DateTime(2026, 03, 27, 13, 00, 00, DateTimeKind.Utc),
            NumberOfPeople = 1,
            PizzaOrderConstraints =
            [
                new PizzaOrderConstraintsModel
                {
                    ConstraintId = vegan.Id,
                    DietaryConstraint = vegan,
                    Count = 1,
                },
            ],
        };
        _dbContext.PizzaOrders.Add(order);
        await _dbContext.SaveChangesAsync();

        var result = await _service.AttendAsync(order.Id, [1], CancellationToken.None);

        result.ShouldNotBeNull();
        result.NumberOfPeople.ShouldBe(2);

        var constraint = result.Constraints.ShouldHaveSingleItem();
        constraint.Name.ShouldBe("Vegan");
        constraint.Count.ShouldBe(2);
    }

    [Fact]
    public async Task GetConstraintsAsync_Returns_All_Constraints()
    {
        SetUtcNow(new DateTime(2026, 03, 27, 12, 00, 00, DateTimeKind.Utc));

        _dbContext.DietaryConstraints.AddRange(
            new DietaryConstraintModel { Id = 1, Name = "Vegan" },
            new DietaryConstraintModel { Id = 2, Name = "Gluten" }
        );
        await _dbContext.SaveChangesAsync();

        var result = await _service.GetConstraintsAsync(CancellationToken.None);

        result.Select(x => x.Name).OrderBy(x => x).ToArray().ShouldBe(["Gluten", "Vegan"]);
    }

    private void SetUtcNow(DateTime utcNow) =>
        _timeProvider.GetUtcNow().Returns(new DateTimeOffset(utcNow));
}
