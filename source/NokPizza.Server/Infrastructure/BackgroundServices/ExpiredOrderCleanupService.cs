using NokPizza.Server.Services.PizzaOrder;

namespace NokPizza.Server.Infrastructure.BackgroundServices;

public class ExpiredOrderCleanupService(
    IServiceScopeFactory scopeFactory,
    ILogger<ExpiredOrderCleanupService> logger
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(15));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IPizzaOrderService>();
                await service.DeleteExpiredAsync(stoppingToken);
                logger.LogInformation("Deleted expired pizza orders");
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "Failed to delete expired pizza orders");
            }
        }
    }
}
