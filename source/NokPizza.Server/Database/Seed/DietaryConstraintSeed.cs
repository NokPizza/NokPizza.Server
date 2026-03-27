using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NokPizza.Server.Database.Models;

namespace NokPizza.Server.Database.Seed;

public class DietaryConstraintSeed : IEntityTypeConfiguration<DietaryConstraintModel>
{
    public void Configure(EntityTypeBuilder<DietaryConstraintModel> builder) =>
        builder.HasData(
            new DietaryConstraintModel { Id = 1, Name = "Vegan" },
            new DietaryConstraintModel { Id = 2, Name = "Vegetarian" },
            new DietaryConstraintModel { Id = 3, Name = "Gluten" },
            new DietaryConstraintModel { Id = 4, Name = "Dairy" },
            new DietaryConstraintModel { Id = 5, Name = "Nuts" },
            new DietaryConstraintModel { Id = 6, Name = "Fish" },
            new DietaryConstraintModel { Id = 7, Name = "Shellfish" },
            new DietaryConstraintModel { Id = 8, Name = "Eggs" }
        );
}
