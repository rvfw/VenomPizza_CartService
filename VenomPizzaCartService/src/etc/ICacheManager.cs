using System.Collections.Concurrent;
using System.Text.Json;
using VenomPizzaCartService.src.dto;

namespace VenomPizzaCartService.src.etc;

public interface ICacheManager
{
    ConcurrentDictionary<int, ProductShortInfoDto> GetProductCache();
    Task AddProductInfo(ProductShortInfoDto info);
    Task UpdateProductInfo(ProductShortInfoDto info);
    Task DeleteProductInfo(int id);
    void SaveSnapshotCallback(object? state);

    Task SaveSnapshot();

    ValueTask DisposeAsync();
}
