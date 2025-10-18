using StackExchange.Redis;
using System.Text.Json;
using VenomPizzaCartService.src.model;
using VenomPizzaCartService.src.providers;

namespace VenomPizzaCartService.src.etc;

public class CacheProvider : ICacheProvider
{
    private readonly IDatabase _database;
    private readonly ILogger<CacheProvider> _logger;
    private static readonly Dictionary<Type, string> _keyPrefix = new()
    {
        [typeof(Cart)] = "cart"
    };

    public CacheProvider(IConnectionMultiplexer redis, ILogger<CacheProvider> logger)
    {
        _database=redis.GetDatabase();
        _logger=logger;
    }
    public async Task<T?> GetAsync<T>(int id)
    {
        try
        {
            var key = GetKey(typeof(T), id);
            var foundedValue = await _database.StringGetAsync(key);
            if (foundedValue.IsNullOrEmpty)
                return default;
            _logger.LogInformation($"Получено значение из редиса {key}");
            return JsonSerializer.Deserialize<T>(foundedValue!);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Не удалось получить в редис значение с Id {id}");
            return default;
        }
    }

    public async Task SetAsync<T>(int id, T value, TimeSpan expiration)
    {
        try
        {
            var key = GetKey(typeof(T), id);
            var json = JsonSerializer.Serialize(value);
            await _database.StringSetAsync(key, json, expiration);
            _logger.LogInformation($"Установлено значение в редисе для {key}");
        }
        catch(Exception ex)
        {
            _logger.LogWarning($"Не удалось записать в редис значение с Id {id}: {ex.Message}");
        }
    }
    public async Task<bool> RemoveAsync<T>(int id)
    {
        try
        {
            var key = GetKey(typeof(T), id);
            var res= await _database.KeyDeleteAsync(key);
            if(res)
                _logger.LogInformation($"Удалено значение в редисе: {key}");
            return res;
        }
        catch(Exception ex)
        {
            _logger.LogError($"Не удалось удалить в редисе значение для Id {id}: {ex.Message}");
            return false;
        }
    }
    private string GetKey(Type type, int id)
    {
        if (!_keyPrefix.TryGetValue(type, out var prefix))
            throw new KeyNotFoundException($"Не найден префикс для редиса для типа {type}");
        return $"{prefix}:{id}";
    }
}
