using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NokPizza.Server.Database.Models;

namespace NokPizza.Server.Database.Configuration;

public class PizzaOrderConstraintsConfiguration : IEntityTypeConfiguration<PizzaOrderConstraints>
{
    public void Configure(EntityTypeBuilder<PizzaOrderConstraints> builder)
    {
        // Composite primary key
        builder.HasKey(x => new { x.PizzaOrderId, x.ConstraintId });

        // Relationship to PizzaOrder
        builder
            .HasOne(x => x.PizzaOrder)
            .WithMany(o => o.PizzaOrderConstraints)
            .HasForeignKey(x => x.PizzaOrderId);

        // Relationship to DietaryConstraint
        builder.HasOne(x => x.DietaryConstraint).WithMany().HasForeignKey(x => x.ConstraintId);
    }
}
