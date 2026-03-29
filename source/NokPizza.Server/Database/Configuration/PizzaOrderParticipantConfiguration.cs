using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NokPizza.Server.Database.Models;

namespace NokPizza.Server.Database.Configuration;

public class PizzaOrderParticipantConfiguration
    : IEntityTypeConfiguration<PizzaOrderParticipantModel>
{
    public void Configure(EntityTypeBuilder<PizzaOrderParticipantModel> builder) =>
        builder
            .HasMany(x => x.PizzaOrderParticipantConstraints)
            .WithOne(x => x.Participant)
            .HasForeignKey(x => x.ParticipantId)
            .OnDelete(DeleteBehavior.Cascade);
}
