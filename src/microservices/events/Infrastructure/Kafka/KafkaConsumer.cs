using Confluent.Kafka;
using Microsoft.Extensions.Options;

namespace events.Infrastructure.Kafka
{
    public class KafkaConsumer
    {
        private readonly string _bootstrapServers;
        private readonly string _groupId;

        private readonly ConsumerConfig _config;
        private IConsumer<Ignore, string> _consumer;
        private readonly List<string> _topics;
        private CancellationTokenSource _cancellationTokenSource;

        public KafkaConsumer(IOptions<KafkaOption> options)
        {
            _bootstrapServers = options.Value.BootstrapServers;
            _groupId = options.Value.GroupId;
        
            _config = new ConsumerConfig
                {
                    BootstrapServers = _bootstrapServers,
                    GroupId = _groupId,
                    AutoOffsetReset = AutoOffsetReset.Earliest,
                    EnableAutoCommit = true,
                    SessionTimeoutMs = 10000,
                    HeartbeatIntervalMs = 3000
                };

                // Список топиков для подписки
            _topics = new List<string>
            {
                "movie-events",
                "user-events",
                "payment-events"
            };
        }

        public async Task StartConsumingAsync()
        {
            try
            {
                // Создание потребителя
                _consumer = new ConsumerBuilder<Ignore, string>(_config)
                    .SetErrorHandler((_, e) => Console.WriteLine($"Ошибка: {e.Reason}"))
                    .Build();

                // Подписка на топики
                _consumer.Subscribe(_topics);

                Console.WriteLine("Потребитель запущен...");

                _cancellationTokenSource = new CancellationTokenSource();
                ConsumeLoop(_cancellationTokenSource.Token);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Критическая ошибка: {ex.Message}");
            }
            finally
            {
                _consumer?.Close();
                Console.WriteLine("Потребитель остановлен.");
            }
        }

        private void ConsumeLoop(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = _consumer.Consume(cancellationToken);

                    // Обработка сообщений по топикам
                    switch (consumeResult.Topic)
                    {
                        case "movie-events":
                            HandleMovieMessage(consumeResult.Message.Value);
                            break;
                        case "user-events":
                            HandleUserMessage(consumeResult.Message.Value);
                            break;
                        case "payment-events":
                            HandlePaymentMessage(consumeResult.Message.Value);
                            break;
                        default:
                            Console.WriteLine($"Неизвестный топик: {consumeResult.Topic}");
                            break;
                    }
                }
                catch (ConsumeException e)
                {
                    Console.WriteLine($"Ошибка при потреблении: {e.Error.Reason}");
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }

        private void HandleMovieMessage(string message)
        {
            Console.WriteLine($"Обработано сообщение из movie-events: {message}");
        }

        private void HandleUserMessage(string message)
        {
            Console.WriteLine($"Обработано сообщение из user-events: {message}");
        }

        private void HandlePaymentMessage(string message)
        {
            Console.WriteLine($"Обработано сообщение из payment-events: {message}");
        }

        public void StopConsuming()
        {
            _cancellationTokenSource?.Cancel();
        }
    }

}
