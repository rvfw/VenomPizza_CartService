
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using System.Runtime.CompilerServices;
using System.Text.Json;
using VenomPizzaCartService.src.dto;
using VenomPizzaCartService.src.model;
using VenomPizzaCartService.src.service;

namespace VenomPizzaCartService.src.Kafka;

public class KafkaConsumerService : BackgroundService
{
    private readonly KafkaSettings _kafkaSettings;
    private readonly IServiceProvider _serviceProvider;
    private readonly IConsumer<string,string> _consumer;
    private readonly ILogger<KafkaConsumerService> _logger;

    public KafkaConsumerService(IOptions<KafkaSettings> kafkaSettings, IServiceProvider serviceProvider,ILogger<KafkaConsumerService> logger)
    {
        _kafkaSettings = kafkaSettings.Value;
        _serviceProvider = serviceProvider;
        _logger = logger;
        var config = new ConsumerConfig
        {
            BootstrapServers = _kafkaSettings.BootstrapServers,
            GroupId = _kafkaSettings.GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
        _consumer = new ConsumerBuilder<string, string>(config).Build();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var topics = new[]
        {
            _kafkaSettings.Topics.ProductUpdated,
            _kafkaSettings.Topics.CartUpdated,
        };
        _consumer.Subscribe(topics);
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var result =await Task.Run(()=> _consumer.Consume(stoppingToken),stoppingToken);
                _logger.LogInformation($"Получено из топика {result.Topic}:\n{result.Message.Value}");
                using (var scope = _serviceProvider.CreateScope())
                {
                    var cartsService = scope.ServiceProvider.GetRequiredService<CartsService>();
                    await ProccessRequestAsync(cartsService, result.Topic, result.Message.Value); ;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка обработки запроса");
            }
        }
    }

    private async Task ProccessRequestAsync(ICartsService cartsService,string topic,string message)
    {
        KafkaEvent<CartProductDto>? dto = JsonSerializer.Deserialize<KafkaEvent<CartProductDto>>(message);
        if (dto == null)
            throw new ArgumentNullException("Пустой json");
        var product = dto.Data??throw new ArgumentException("Пустой объект с информацией");
        if (topic == _kafkaSettings.Topics.CartUpdated)
        {
            if(dto.EventType=="product_added")
                await cartsService.AddProductToCart(product.CartId, product.Id, product.Quantity);
            else if (dto.EventType == "product_updated")
                await cartsService.UpdateProductQuantity(product.CartId, product.Id, product.Quantity);
            else if (dto.EventType == "product_deleted")
                await cartsService.DeleteProductInCart(product.CartId, product.Id);
            else
                throw new ArgumentException("Неверный тип ивента, есть только: product_added, product_updated, product_deleted");
        }
    }
}
