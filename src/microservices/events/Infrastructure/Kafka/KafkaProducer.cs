using Confluent.Kafka;
using Microsoft.Extensions.Options;

namespace events.Infrastructure.Kafka
{
    public class KafkaProducer
    {
        private readonly ProducerConfig _config;

        public KafkaProducer(IOptions<KafkaOption> options)
        {
            _config = new ProducerConfig
            {
                BootstrapServers = options.Value.BootstrapServers,
                MessageMaxBytes = 157286400,
                RequestTimeoutMs = 30000
            };
        }

        public async Task ProduceAsync(string topic, string message)
        {
            using (var producer = new ProducerBuilder<Null, string>(_config).Build())
            {
                try
                {
                    var deliveryReport = await producer.ProduceAsync(topic, new Message<Null, string> { Value = message });
                    Console.WriteLine($"Message delivered to: {deliveryReport.TopicPartitionOffset}");
                }
                catch (ProduceException<Null, string> e)
                {
                    Console.WriteLine($"Produce failed: {e.Error.Reason}");
                }
            }
        }

    }
}
