namespace VenomPizzaCartService.src.dto;

public class KafkaEvent<T>
{
    public string EventType { get; set; }
    public T? Data { get; set; }
}