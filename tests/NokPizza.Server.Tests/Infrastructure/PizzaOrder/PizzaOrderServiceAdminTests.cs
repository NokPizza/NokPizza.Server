using NokPizza.Server.Database.Models;
using NokPizza.Server.Infrastructure.Dto.PizzaOrderParticipant;
using Shouldly;

namespace NokPizza.Server.Tests.Infrastructure.PizzaOrder;

public class PizzaOrderServiceAdminTests : PizzaOrderServiceTestBase
{
    [Fact]
    public async Task GetAdminAccessAsync_Returns_Current_Password_State()
    {
        var id = await CreateOrderAsync(adminPassword: "admin-secret");

        var access = await Service.GetAdminAccessAsync(
            id,
            "wrong-password",
            CancellationToken.None
        );

        access.ShouldNotBeNull();
        access.IsPasswordValid.ShouldBeFalse();
    }

    [Fact]
    public async Task GetAdmin_Returns_Creator_And_Participant_Details()
    {
        await SeedConstraintsAsync(new DietaryConstraintModel { Id = 1, Name = "Vegan" });

        var id = await CreateOrderAsync(
            creatorEmail: "creator@example.com",
            adminPassword: "admin-secret",
            participantPassword: "party-secret"
        );

        var participantAccess = await GetParticipantAccessAsync(id, "party-secret");
        await Service.SaveParticipantAsync(
            participantAccess,
            new ParticipantAttendRequestDto("guest@example.com", "party-secret", [1]),
            CancellationToken.None
        );
        ClearTracking();

        var adminAccess = await GetAdminAccessAsync(id, "admin-secret");
        adminAccess.IsPasswordValid.ShouldBeTrue();

        var result = Service.GetAdmin(adminAccess);

        result.CreatorEmail.ShouldBe("creator@example.com");
        result.NumberOfPeople.ShouldBe(1);
        result.Participants.ShouldHaveSingleItem().Email.ShouldBe("guest@example.com");
        result.Constraints.ShouldHaveSingleItem().ShouldBe(new("Vegan", 1));
    }

    [Fact]
    public async Task DeleteParticipantAsync_Removes_Participant_And_Returns_True()
    {
        await SeedConstraintsAsync(new DietaryConstraintModel { Id = 1, Name = "Vegan" });

        var id = await CreateOrderAsync(
            adminPassword: "admin-secret",
            participantPassword: "party-secret"
        );

        var participantAccess = await GetParticipantAccessAsync(id, "party-secret");
        await Service.SaveParticipantAsync(
            participantAccess,
            new ParticipantAttendRequestDto("guest@example.com", "party-secret", [1]),
            CancellationToken.None
        );
        ClearTracking();

        var participantId = DbContext.PizzaOrderParticipants.Single().Id;
        var adminAccess = await GetAdminAccessAsync(id, "admin-secret");
        var deleted = await Service.DeleteParticipantAsync(
            adminAccess,
            participantId,
            CancellationToken.None
        );

        deleted.ShouldBeTrue();
        DbContext.PizzaOrderParticipants.ShouldBeEmpty();
    }

    [Fact]
    public async Task FindByEmailAsync_Returns_Active_Orders_For_Creator_And_Participant()
    {
        await SeedConstraintsAsync(new DietaryConstraintModel { Id = 1, Name = "Vegan" });

        var creatorOrderId = await CreateOrderAsync(
            creatorEmail: "shared@example.com",
            adminPassword: "admin-secret"
        );

        var participantOrderId = await CreateOrderAsync(
            creatorEmail: "someoneelse@example.com",
            adminPassword: "admin-secret",
            participantPassword: "party-secret",
            endTime: new DateTime(2026, 03, 27, 19, 00, 00, DateTimeKind.Utc),
            rsvpDeadline: new DateTime(2026, 03, 27, 17, 00, 00, DateTimeKind.Utc)
        );

        var participantAccess = await GetParticipantAccessAsync(participantOrderId, "party-secret");
        await Service.SaveParticipantAsync(
            participantAccess,
            new ParticipantAttendRequestDto("shared@example.com", "party-secret", [1]),
            CancellationToken.None
        );
        ClearTracking();

        await CreateOrderAsync(
            creatorEmail: "shared@example.com",
            adminPassword: "admin-secret",
            endTime: new DateTime(2026, 03, 27, 11, 00, 00, DateTimeKind.Utc),
            rsvpDeadline: new DateTime(2026, 03, 27, 10, 00, 00, DateTimeKind.Utc)
        );

        var result = (
            await Service.FindByEmailAsync(" Shared@Example.com ", CancellationToken.None)
        )
            .OrderBy(x => x.EndTime)
            .ToArray();

        result.Length.ShouldBe(2);

        result[0].Id.ShouldBe(creatorOrderId);
        result[0].IsCreator.ShouldBeTrue();
        result[0].IsParticipant.ShouldBeFalse();

        result[1].Id.ShouldBe(participantOrderId);
        result[1].IsCreator.ShouldBeFalse();
        result[1].IsParticipant.ShouldBeTrue();
    }
}
