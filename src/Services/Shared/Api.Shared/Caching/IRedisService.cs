using System.Text;
using Api.Shared.Serializer;
using StackExchange.Redis;

namespace Api.Shared.Caching;

public interface IRedisService
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);
    Task<bool> DeleteAsync(string key);
    IServer GetServer();
}


public class RedisService : IRedisService
{
    private readonly IConnectionMultiplexer _redisConnection;
    private readonly IDatabase _database;
    private readonly ISerializerService _serializer;

    public RedisService(IConnectionMultiplexer redisConnection, ISerializerService serializer)
    {
        _redisConnection = redisConnection ?? throw new ArgumentNullException(nameof(redisConnection));
        _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        _database = _redisConnection.GetDatabase(); // Get the default Redis database
    }

    // Get a cached item from Redis and deserialize it
    public async Task<T?> GetAsync<T>(string key)
    {
        var value = await _database.StringGetAsync(key);

        if (value.IsNullOrEmpty)
        {
            return default;
        }

        // Deserialize the byte[] back into an object
        return Deserialize<T>(value);
    }

    // Set a cache item in Redis with optional expiration
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        // Serialize the object to a byte[]
        var serializedValue = Serialize(value);

        // Store the byte[] in Redis
        await _database.StringSetAsync(key, serializedValue, expiry);
    }

    // Delete a cache item from Redis
    public async Task<bool> DeleteAsync(string key)
    {
        return await _database.KeyDeleteAsync(key);
    }

    // Helper methods to serialize and deserialize objects
    private byte[] Serialize<T>(T item) =>
        Encoding.Default.GetBytes(_serializer.Serialize(item));

    private T Deserialize<T>(byte[] cachedData) =>
        _serializer.Deserialize<T>(Encoding.Default.GetString(cachedData));
    
    public IServer GetServer()
    {
        var endpoints = _redisConnection.GetEndPoints();
        
        return _redisConnection.GetServer(endpoints.First());
    }
}
