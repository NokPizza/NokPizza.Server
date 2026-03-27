using Microsoft.EntityFrameworkCore;
using NokPizza.Server.Database;

var builder = WebApplication.CreateBuilder(args);

// Add controllers and OpenApi
builder.Services.AddControllers();
builder.Services.AddOpenApi();

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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();

app.Run();
