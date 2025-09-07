using System.Collections.Concurrent;
using System.Text.Json;
using VenomPizzaCartService.src.dto;

namespace VenomPizzaCartService.src.etc;

public interface ICacheManager
{
    ConcurrentDictionary<int, ProductShortInfoDto> GetProductCache();
    void AddProductInfo(ProductShortInfoDto info);
    void UpdateProductInfo(ProductShortInfoDto info);
    void DeleteProductInfo(int id);
    void SaveSnapshotCallback(object? state);

    Task SaveSnapshot();

    ValueTask DisposeAsync();
}
