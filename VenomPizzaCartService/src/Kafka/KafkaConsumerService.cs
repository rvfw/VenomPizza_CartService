
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
    private readonly Dictionary<string, Func<ICartsService, CartProductDto, Task>> _cartEventHandlers;
    private readonly Dictionary<string, Action<ICartsService, ProductShortInfoDto>> _productEventHandlers;

    public KafkaConsumerService(IOptions<KafkaSettings> kafkaSettings, IServiceProvider serviceProvider,ILogger<KafkaConsumerService> logger)
    {
        _cartEventHandlers = new()
        {
            ["product_added"] = (service, product) => service.AddProductToCart(product.CartId, product.ProductId,product.PriceId, product.Quantity),
            ["product_updated"] = (service, product) => service.UpdateProductQuantity(product.CartId, product.ProductId, product.PriceId, product.Quantity),
            ["product_deleted"] = (service, product) => service.DeleteProductInCart(product.CartId, product.PriceId, product.ProductId),
        };

        _productEventHandlers = new()
        {
            ["product_added"]=(service,product)=>service.AddProductInfo(product),
            ["product_updated"] = (service, product) => service.UpdateProductInfo(product),
            ["product_deleted"] = (service, product) => service.DeleteProductInfo(product),
        };

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
        await using var scope = _serviceProvider.CreateAsyncScope();
        var cartsService = scope.ServiceProvider.GetRequiredService<CartsService>();
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var result = _consumer.Consume(TimeSpan.FromMilliseconds(1000));
                if (result?.IsPartitionEOF ?? true)
                {
                    await Task.Delay(100, stoppingToken);
                    continue;
                }
                //var result =await Task.Run(()=> _consumer.Consume(stoppingToken),stoppingToken);
                _logger.LogInformation($"Получено из топика {result.Topic}:\n{result.Message.Value}");
                await ProccessRequestAsync(cartsService, result.Topic, result.Message.Value); ;
            }
            catch(TaskCanceledException ex)
            {
                _logger.LogDebug(ex, "Кафка консьюмер остановлен");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка обработки запроса");
            }
        }
    }

    private async Task ProccessRequestAsync(ICartsService cartsService,string topic,string message)
    {
        if (topic == _kafkaSettings.Topics.CartUpdated)
        {
            KafkaEvent<CartProductDto>? dto = JsonSerializer.Deserialize<KafkaEvent<CartProductDto>>(message);
            if (dto == null)
                throw new ArgumentNullException("Пустой json");
            var product = dto.Data ?? throw new ArgumentException("Пустой объект с информацией");
            if (_cartEventHandlers.TryGetValue(dto.EventType, out var func))
                await func(cartsService, product);
            else
                throw new ArgumentException("Неверный тип ивента, есть только: product_added, product_updated, product_deleted");
        }
        else if (topic == _kafkaSettings.Topics.ProductUpdated)
        {
            KafkaEvent<ProductShortInfoDto>? dto = JsonSerializer.Deserialize<KafkaEvent<ProductShortInfoDto>>(message);
            if (dto == null)
                throw new ArgumentNullException("Пустой json");
            var product = dto.Data ?? throw new ArgumentException("Пустой объект с информацией");
            if (_productEventHandlers.TryGetValue(dto.EventType, out var func))
                func(cartsService, product);
            else
                throw new ArgumentException("Неверный тип ивента, есть только: product_added, product_updated, product_deleted");
        }
    }
}
