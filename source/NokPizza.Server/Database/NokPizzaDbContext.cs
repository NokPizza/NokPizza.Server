using Microsoft.EntityFrameworkCore;
using NokPizza.Server.Database.Models;

namespace NokPizza.Server.Database;

public class NokPizzaDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<PizzaOrder> PizzaOrders => Set<PizzaOrder>();
    public DbSet<DietaryConstraint> DietaryConstraints => Set<DietaryConstraint>();
    public DbSet<PizzaOrderConstraints> PizzaOrderConstraints => Set<PizzaOrderConstraints>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(NokPizzaDbContext).Assembly);
    }
}
