namespace VenomPizzaCartService.src.providers;

public interface ICacheProvider
{
    Task<T?> GetAsync<T>(int key);
    Task SetAsync<T>(int key, T value, TimeSpan expiration);
    Task<bool> RemoveAsync<T>(int key);
}
