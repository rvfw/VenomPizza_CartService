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
    public async Task<T?> GetAsync<T>(int key)
    {
        try
        {
            var foundedValue = await _database.StringGetAsync(key.ToString());
            if (foundedValue.IsNullOrEmpty)
                return default;
            return JsonSerializer.Deserialize<T>(foundedValue!);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Не удалось получить в редис значение с ключом {key}: {ex.Message}");
            return default;
        }
    }

    public async Task SetAsync<T>(int key, T value, TimeSpan expiration)
    {
        try
        {
            var json = JsonSerializer.Serialize(value);
            await _database.StringSetAsync($"{value.GetType().Name}:{key}", json, expiration);
        }
        catch(Exception ex)
        {
            _logger.LogWarning($"Не удалось записать в редис значение с ключом {key}: {ex.Message}");
        }
    }
    public async Task<bool> RemoveAsync(int key)
    {
        try
        {
            return await _database.KeyDeleteAsync(key.ToString());
        }
        catch(Exception ex)
        {
            _logger.LogError($"Не удалось удалить в редисе значения для ключа {key}: {ex.Message}");
            return false;
        }
    }
}
