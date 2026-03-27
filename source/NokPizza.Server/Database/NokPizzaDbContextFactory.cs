using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace NokPizza.Server.Database;

public class NokPizzaDbContextFactory : IDesignTimeDbContextFactory<NokPizzaDbContext>
{
    public NokPizzaDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder()
            .UseSqlServer("Server=localhost;Database=NokPizza;Trusted_Connection=True;")
            .Options;

        return new NokPizzaDbContext(options);
    }
}
