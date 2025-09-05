
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
            _kafkaSettings.Topics.ProductAddedInCart,
            _kafkaSettings.Topics.ProductQuantityUpdated,
            _kafkaSettings.Topics.ProductDeletedInCart,
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
        CartProductDto? dto = JsonSerializer.Deserialize<CartProductDto>(message);
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));
        if (topic == _kafkaSettings.Topics.ProductAddedInCart)
            await cartsService.AddProductToCart(dto.CartId,dto.Id,dto.Quantity);
        else if(topic == _kafkaSettings.Topics.ProductQuantityUpdated)
            await cartsService.UpdateProductQuantity(dto.CartId,dto.Id,dto.Quantity);
        else if(topic==_kafkaSettings.Topics.ProductDeletedInCart)
            await cartsService.DeleteProductInCart(dto.CartId,dto.Id);
    }
}
