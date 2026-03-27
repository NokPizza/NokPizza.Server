using Microsoft.EntityFrameworkCore;
using NokPizza.Server.Database.Models;

namespace NokPizza.Server.Database;

public class NokPizzaDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<PizzaOrderModel> PizzaOrders => Set<PizzaOrderModel>();
    public DbSet<DietaryConstraintModel> DietaryConstraints => Set<DietaryConstraintModel>();
    public DbSet<PizzaOrderConstraintsModel> PizzaOrderConstraints =>
        Set<PizzaOrderConstraintsModel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(NokPizzaDbContext).Assembly);
    }
}
