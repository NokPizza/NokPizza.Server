using Microsoft.EntityFrameworkCore;
using NokPizza.Server.Database.Models;
using NokPizza.Server.Infrastructure.Dto.PizzaOrderParticipant;
using Shouldly;

namespace NokPizza.Server.Tests.Infrastructure.PizzaOrder;

public class PizzaOrderServiceCoreTests : PizzaOrderServiceTestBase
{
    [Fact]
    public async Task CreateAsync_Saves_Normalized_Creator_Email_And_Hashed_Passwords()
    {
        var id = await CreateOrderAsync(
            creatorEmail: " Creator@Example.com ",
            adminPassword: "admin-secret",
            participantPassword: "party-secret"
        );

        var order = await DbContext.PizzaOrders.SingleAsync(x => x.Id == id);

        order.CreatorEmail.ShouldBe("Creator@Example.com");
        order.NormalizedCreatorEmail.ShouldBe("creator@example.com");
        order.AdminPasswordHash.ShouldNotBe("admin-secret");
        order.ParticipantPasswordHash.ShouldNotBeNull();
        order.ParticipantPasswordHash.ShouldNotBe("party-secret");
        PasswordService.VerifyPassword(order.AdminPasswordHash, "admin-secret").ShouldBeTrue();
        PasswordService
            .VerifyPassword(order.ParticipantPasswordHash, "party-secret")
            .ShouldBeTrue();
    }

    [Fact]
    public async Task GetAsync_Returns_Null_For_Expired_Order()
    {
        var id = await CreateOrderAsync(
            endTime: new DateTime(2026, 03, 27, 11, 00, 00, DateTimeKind.Utc),
            rsvpDeadline: new DateTime(2026, 03, 27, 10, 00, 00, DateTimeKind.Utc)
        );

        var result = await Service.GetAsync(id, CancellationToken.None);

        result.ShouldBeNull();
    }

    [Fact]
    public async Task GetAsync_Returns_Aggregated_Participant_Data_For_Active_Order()
    {
        await SeedConstraintsAsync(
            new DietaryConstraintModel { Id = 1, Name = "Vegan" },
            new DietaryConstraintModel { Id = 2, Name = "Gluten" }
        );

        var id = await CreateOrderAsync(participantPassword: "party-secret");

        var firstAccess = await GetParticipantAccessAsync(id, "party-secret");
        await Service.SaveParticipantAsync(
            firstAccess,
            new ParticipantAttendRequestDto("first@example.com", "party-secret", [1]),
            CancellationToken.None
        );
        ClearTracking();

        var secondAccess = await GetParticipantAccessAsync(id, "party-secret");
        await Service.SaveParticipantAsync(
            secondAccess,
            new ParticipantAttendRequestDto("second@example.com", "party-secret", [1, 2]),
            CancellationToken.None
        );
        ClearTracking();

        var result = await Service.GetAsync(id, CancellationToken.None);

        result.ShouldNotBeNull();
        result.NumberOfPeople.ShouldBe(2);
        result.ParticipantPasswordRequired.ShouldBeTrue();
        result
            .Constraints.OrderBy(x => x.Name)
            .ToArray()
            .ShouldBe([new("Gluten", 1), new("Vegan", 2)]);
    }
}
