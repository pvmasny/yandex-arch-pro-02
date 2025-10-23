
using events.Infrastructure.Kafka;

namespace events.Workers;
public class FakeConsumerWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public FakeConsumerWorker(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(10000);
        using var scope = _serviceProvider.CreateScope();
        if (scope == null)
        {
            return;
        }

        var service = scope.ServiceProvider.GetRequiredService<KafkaConsumer>();
        await service.StartConsumingAsync();
    }

}