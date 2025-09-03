namespace VenomPizzaCartService.src.Kafka;

public class KafkaSettings
{
    public string BootstrapServers { get; set; }
    public string GroupId { get; set; }
    public KafkaTopics Topics { get; set; }
}
public class KafkaTopics
{
    public string ProductAddedInCart { get; set; }
    public string ProductQuantityUpdated { get; set; }
    public string ProductDeletedInCart { get; set; }
}
