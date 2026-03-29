using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NokPizza.Server.Database.Models;

namespace NokPizza.Server.Database.Configuration;

public class PizzaOrderParticipantConstraintConfiguration
    : IEntityTypeConfiguration<PizzaOrderParticipantConstraintModel>
{
    public void Configure(EntityTypeBuilder<PizzaOrderParticipantConstraintModel> builder)
    {
        builder
            .HasOne(x => x.Participant)
            .WithMany(x => x.PizzaOrderParticipantConstraints)
            .HasForeignKey(x => x.ParticipantId);

        builder.HasOne(x => x.DietaryConstraint).WithMany().HasForeignKey(x => x.ConstraintId);
    }
}
