using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Api.Shared.Caching;

public class LocalCacheService : ICacheService
{
    private readonly int _cacheExpirationMinutes; // 30 minutes
    private readonly IMemoryCache _cache;
    private readonly ILogger<LocalCacheService> _logger;

    public LocalCacheService(IMemoryCache memoryCache, IConfiguration configuration, ILogger<LocalCacheService> logger)
    {
        _cache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _logger = logger;
        _cacheExpirationMinutes = configuration?.GetValue<int>("CacheExpirationMinutesForCollectionService") 
                                  ?? throw new ArgumentNullException(nameof(configuration));
    }

    public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> createFunc, TimeSpan? slidingExpiration = null, TimeSpan? absoluteExpirationRelativeToNow = null)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentNullException(nameof(key));

        if (createFunc == null)
            throw new ArgumentNullException(nameof(createFunc));

        if (_cache.TryGetValue(key, out T result)) 
            return result;

        try
        {
            result = await createFunc();
        }
        catch (Exception ex)
        {
            // Handle the exception according to your application's needs
            throw new Exception("An error occurred while executing createFunc.", ex);
        }

        var cacheEntryOptions = new MemoryCacheEntryOptions();

        if (slidingExpiration.HasValue)
            cacheEntryOptions.SetSlidingExpiration(slidingExpiration.Value);
        else
            cacheEntryOptions.SetSlidingExpiration(TimeSpan.FromMinutes(_cacheExpirationMinutes / 2)); // Half of cache expiration

        if (absoluteExpirationRelativeToNow.HasValue)
            cacheEntryOptions.SetAbsoluteExpiration(absoluteExpirationRelativeToNow.Value);
        else
            cacheEntryOptions.SetAbsoluteExpiration(TimeSpan.FromMinutes(_cacheExpirationMinutes));

        _cache.Set(key, result, cacheEntryOptions);

        return result;
    }

    public T? Get<T>(string key) =>
        _cache.Get<T>(key);

    public Task<T?> GetAsync<T>(string key, CancellationToken token = default) =>
        Task.FromResult(Get<T>(key));

    public void Refresh(string key) =>
        _cache.TryGetValue(key, out _);

    public Task RefreshAsync(string key, CancellationToken token = default)
    {
        Refresh(key);
        return Task.CompletedTask;
    }

    public void Remove(string key) =>
        _cache.Remove(key);

    public Task RemoveAsync(string key, CancellationToken token = default)
    {
        Remove(key);
        return Task.CompletedTask;
    }

    public void Set<T>(string key, T value, TimeSpan? slidingExpiration = null)
    {
        // TODO: add to appsettings?
        slidingExpiration ??= TimeSpan.FromMinutes(10); // Default expiration time of 10 minutes.

        _cache.Set(key, value, new MemoryCacheEntryOptions { SlidingExpiration = slidingExpiration });
        _logger.LogDebug($"Added to Cache : {key}", key);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? slidingExpiration = null, CancellationToken token = default)
    {
        Set(key, value, slidingExpiration);
        return Task.CompletedTask;
    }
}
