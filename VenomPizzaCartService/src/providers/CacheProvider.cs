using StackExchange.Redis;
using System.Text.Json;
using VenomPizzaCartService.src.providers;

namespace VenomPizzaCartService.src.etc;

public class CacheProvider : ICacheProvider
{
    private readonly IDatabase _database;
    private readonly ILogger<CacheProvider> _logger;

    public CacheProvider(IConnectionMultiplexer redis, ILogger<CacheProvider> logger)
    {
        _database=redis.GetDatabase();
        _logger=logger;
    }
    public async Task<T?> GetAsync<T>(int id)
    {
        var key= $"{ typeof(T).Name }:{ id}";
        try
        {
            var foundedValue = await _database.StringGetAsync(key);
            if (foundedValue.IsNullOrEmpty)
                return default;
            _logger.LogInformation($"Получено значение из редиса {key}");
            return JsonSerializer.Deserialize<T>(foundedValue!);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Не удалось получить в редис значение {key}");
            return default;
        }
    }

    public async Task SetAsync<T>(int id, T value, TimeSpan expiration)
    {
        var key = $"{typeof(T).Name}:{id}";
        try
        {
            var json = JsonSerializer.Serialize(value);
            await _database.StringSetAsync(key, json, expiration);
            _logger.LogInformation($"Установлено значение в редисе для {key}");
        }
        catch(Exception ex)
        {
            _logger.LogWarning($"Не удалось записать в редис значение с ключом {key}: {ex.Message}");
        }
    }
    public async Task<bool> RemoveAsync<T>(int id)
    {
        var key = $"{typeof(T).Name}:{id}";
        try
        {
            var res= await _database.KeyDeleteAsync(key);
            if(res)
                _logger.LogInformation($"Удалено значение в редисе: {key}");
            return res;
        }
        catch(Exception ex)
        {
            _logger.LogError($"Не удалось удалить в редисе значения для ключа {key}: {ex.Message}");
            return false;
        }
    }
}
