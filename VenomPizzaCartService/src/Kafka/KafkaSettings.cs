namespace VenomPizzaCartService.src.Kafka;

public class KafkaSettings
{
    public string BootstrapServers { get; set; }
    public string GroupId { get; set; }
    public KafkaTopics Topics { get; set; }
}
public class KafkaTopics
{
    public string ProductUpdated { get; set; }
    public string CartUpdated { get; set; }
    public string OrderRequestCreated { get; set; }
}
