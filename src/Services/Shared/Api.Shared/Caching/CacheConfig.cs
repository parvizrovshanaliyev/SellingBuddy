namespace Api.Shared.Caching;

public class CacheConfig
{
    public bool UseDistributedCache { get; set; }
    public bool PreferRedis { get; set; }
    public string? RedisURL { get; set; }
}