using Microsoft.EntityFrameworkCore;
using NokPizza.Server.Database.Models;
using NokPizza.Server.Infrastructure.Dto.PizzaOrderParticipant;
using Shouldly;

namespace NokPizza.Server.Tests.Infrastructure.PizzaOrder;

public class PizzaOrderServiceParticipantTests : PizzaOrderServiceTestBase
{
    [Fact]
    public async Task GetParticipantAccessAsync_Returns_Current_Password_And_Rsvp_State()
    {
        var id = await CreateOrderAsync(
            participantPassword: "party-secret",
            rsvpDeadline: new DateTime(2026, 03, 27, 11, 00, 00, DateTimeKind.Utc)
        );

        var access = await Service.GetParticipantAccessAsync(
            id,
            "wrong-password",
            CancellationToken.None
        );

        access.ShouldNotBeNull();
        access.IsPasswordValid.ShouldBeFalse();
        access.IsRsvpOpen.ShouldBeFalse();
    }

    [Fact]
    public async Task SaveParticipantAsync_Adds_New_Participant_With_Constraints()
    {
        await SeedConstraintsAsync(
            new DietaryConstraintModel { Id = 1, Name = "Vegan" },
            new DietaryConstraintModel { Id = 2, Name = "Gluten" }
        );

        var id = await CreateOrderAsync(participantPassword: "party-secret");
        var access = await GetParticipantAccessAsync(id, "party-secret");

        var result = await Service.SaveParticipantAsync(
            access,
            new ParticipantAttendRequestDto(" User@Example.com ", "party-secret", [1, 2]),
            CancellationToken.None
        );

        result.NumberOfPeople.ShouldBe(1);
        result
            .Constraints.OrderBy(x => x.Name)
            .ToArray()
            .ShouldBe([new("Gluten", 1), new("Vegan", 1)]);

        var participant = await DbContext
            .PizzaOrderParticipants.Include(x => x.PizzaOrderParticipantConstraints)
            .SingleAsync();
        participant.Email.ShouldBe("User@Example.com");
        participant.NormalizedEmail.ShouldBe("user@example.com");
        participant.PizzaOrderParticipantConstraints.Count.ShouldBe(2);
    }

    [Fact]
    public async Task SaveParticipantAsync_Updates_Existing_Participant_Without_Duplicating_Row()
    {
        await SeedConstraintsAsync(
            new DietaryConstraintModel { Id = 1, Name = "Vegan" },
            new DietaryConstraintModel { Id = 2, Name = "Gluten" }
        );

        var id = await CreateOrderAsync(participantPassword: "party-secret");

        var firstAccess = await GetParticipantAccessAsync(id, "party-secret");
        await Service.SaveParticipantAsync(
            firstAccess,
            new ParticipantAttendRequestDto("user@example.com", "party-secret", [1]),
            CancellationToken.None
        );
        ClearTracking();

        var secondAccess = await GetParticipantAccessAsync(id, "party-secret");
        var result = await Service.SaveParticipantAsync(
            secondAccess,
            new ParticipantAttendRequestDto("user@example.com", "party-secret", [2]),
            CancellationToken.None
        );

        result.NumberOfPeople.ShouldBe(1);
        result.Constraints.ShouldHaveSingleItem().ShouldBe(new("Gluten", 1));
        DbContext.PizzaOrderParticipants.Count().ShouldBe(1);
    }

    [Fact]
    public async Task RemoveParticipantAsync_Removes_Participant_And_Returns_True()
    {
        await SeedConstraintsAsync(new DietaryConstraintModel { Id = 1, Name = "Vegan" });

        var id = await CreateOrderAsync(participantPassword: "party-secret");
        var saveAccess = await GetParticipantAccessAsync(id, "party-secret");
        await Service.SaveParticipantAsync(
            saveAccess,
            new ParticipantAttendRequestDto("user@example.com", "party-secret", [1]),
            CancellationToken.None
        );
        ClearTracking();

        var removeAccess = await GetParticipantAccessAsync(id, "party-secret");
        var removed = await Service.RemoveParticipantAsync(
            removeAccess,
            "user@example.com",
            CancellationToken.None
        );

        removed.ShouldBeTrue();
        DbContext.PizzaOrderParticipants.ShouldBeEmpty();
    }

    [Fact]
    public async Task RemoveParticipantAsync_Returns_False_When_Participant_Does_Not_Exist()
    {
        var id = await CreateOrderAsync(participantPassword: "party-secret");
        var access = await GetParticipantAccessAsync(id, "party-secret");

        var removed = await Service.RemoveParticipantAsync(
            access,
            "missing@example.com",
            CancellationToken.None
        );

        removed.ShouldBeFalse();
    }
}
