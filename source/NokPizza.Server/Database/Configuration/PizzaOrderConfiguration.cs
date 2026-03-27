using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NokPizza.Server.Database.Models;

namespace NokPizza.Server.Database.Configuration;

public class PizzaOrderConfiguration : IEntityTypeConfiguration<PizzaOrderModel>
{
    public void Configure(EntityTypeBuilder<PizzaOrderModel> builder) =>
        builder
            .HasMany(x => x.PizzaOrderConstraints)
            .WithOne(x => x.PizzaOrder)
            .HasForeignKey(x => x.PizzaOrderId)
            .OnDelete(DeleteBehavior.Cascade);
}
