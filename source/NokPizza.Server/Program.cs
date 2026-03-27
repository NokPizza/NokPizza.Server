using Microsoft.EntityFrameworkCore;
using NokPizza.Server.Database;
using NokPizza.Server.Infrastructure.BackgroundServices;
using NokPizza.Server.Infrastructure.PizzaOrder;
using NokPizza.Server.Services.PizzaOrder;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add controllers and OpenApi
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddTransient<IPizzaOrderService, PizzaOrderService>();
builder.Services.AddHostedService<ExpiredOrderCleanupService>();

builder.Services.AddDbContext<NokPizzaDbContext>(options =>
    options.UseSqlServer(
        DatabaseConnectionStringResolver.GetRequiredConnectionString(builder.Configuration)
    )
);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<NokPizzaDbContext>();
    await db.Database.MigrateAsync();
}

// Configure OpenApi
app.MapOpenApi();
app.MapScalarApiReference();
app.MapGet("/", () => Results.Redirect("/scalar/v1")).ExcludeFromDescription();

app.MapControllers();

app.Run();
