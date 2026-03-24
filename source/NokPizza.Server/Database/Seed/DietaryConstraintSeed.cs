using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NokPizza.Server.Database.Models;

namespace NokPizza.Server.Database.Seed;

public class DietaryConstraintSeed : IEntityTypeConfiguration<DietaryConstraint>
{
    public void Configure(EntityTypeBuilder<DietaryConstraint> builder) =>
        builder.HasData(
            new DietaryConstraint { Id = 1, Name = "Vegan" },
            new DietaryConstraint { Id = 2, Name = "Vegetarian" },
            new DietaryConstraint { Id = 3, Name = "Gluten" },
            new DietaryConstraint { Id = 4, Name = "Dairy" },
            new DietaryConstraint { Id = 5, Name = "Nuts" },
            new DietaryConstraint { Id = 6, Name = "Fish" },
            new DietaryConstraint { Id = 7, Name = "Shellfish" },
            new DietaryConstraint { Id = 8, Name = "Eggs" }
        );
}
