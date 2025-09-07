using System.Collections.Concurrent;
using System.Text.Json;
using VenomPizzaCartService.src.dto;

namespace VenomPizzaCartService.src.etc;

public class CacheManager : ICacheManager, IAsyncDisposable
{
    private readonly ILogger<CacheManager> _logger;
    private ConcurrentDictionary<int, ProductShortInfoDto> productsCache = new();
    private readonly CloudStorage _cloudStorage;
    private readonly Timer _snapshotTimer;
    public CacheManager(ILogger<CacheManager> logger)
    {
        _logger = logger;
        _cloudStorage = new CloudStorage();
        var json = _cloudStorage.ReadSnapshot().Result;
        try
        {
            productsCache = JsonSerializer.Deserialize<ConcurrentDictionary<int, ProductShortInfoDto>>(json);
        }
        catch (Exception ex) { _logger.LogError("Не удалось загрузить снапшот продуктов"); }
        productsCache = productsCache == null ? new() : productsCache;
        _logger.LogInformation($"Загружено в кэш {productsCache.Keys.Count()} товаров");
        _snapshotTimer = new Timer(SaveSnapshotCallback, null, TimeSpan.FromMinutes(60), TimeSpan.FromMinutes(60));
    }

    public ConcurrentDictionary<int, ProductShortInfoDto> GetProductCache()
    {
        return productsCache;
    }

    public void AddProductInfo(ProductShortInfoDto product)
    {
        if (productsCache.ContainsKey(product.Id))
            throw new ArgumentException($"Продукт с Id {product.Id} уже существует");
        productsCache[product.Id] = product;
    }

    public void UpdateProductInfo(ProductShortInfoDto info)
    {
        if (productsCache.TryGetValue(info.Id, out var foundedProduct))
            productsCache[info.Id] = info;
        else
            throw new KeyNotFoundException($"Продукт с Id {info.Id} не найден в кэше");
    }

    public void DeleteProductInfo(int id)
    {
        if (!productsCache.TryGetValue(id, out var foundedProduct))
            throw new KeyNotFoundException($"Продукт с ID {id} не найден");
        productsCache.Remove(id, out var _);
    }

    public async void SaveSnapshotCallback(object? state)
    {
        try
        {
            await SaveSnapshot();
        }
        catch (Exception ex) { }
    }

    public async Task SaveSnapshot()
    {
        try
        {
            var json = JsonSerializer.Serialize(productsCache);
            await _cloudStorage.UploadSnapshotAsync(json);
            _logger.LogDebug("Снапшот продуктов сохранен");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при сохранении снапшота продуктов");
        }
    }

    public async ValueTask DisposeAsync()
    {
        _snapshotTimer?.Change(Timeout.Infinite, 0);
        _snapshotTimer?.Dispose();
        await SaveSnapshot();
    }
}
