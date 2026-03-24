using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NokPizza.Server.Database.Models;

namespace NokPizza.Server.Database.Configuration;

public class PizzaOrderConfiguration : IEntityTypeConfiguration<PizzaOrder>
{
    public void Configure(EntityTypeBuilder<PizzaOrder> builder) =>
        builder
            .HasMany(x => x.PizzaOrderConstraints)
            .WithOne(x => x.PizzaOrder)
            .HasForeignKey(x => x.PizzaOrderId)
            .OnDelete(DeleteBehavior.Cascade);
}
