namespace events.Infrastructure.Kafka
{
    public class KafkaOption
    {
        public const string Position = "Kafka";
        public string BootstrapServers { get; set; }

        public string GroupId { get; set; }
    }
}
