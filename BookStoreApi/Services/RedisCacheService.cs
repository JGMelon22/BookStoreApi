using System.Text.Json;
using BookStoreApi.Interfaces;
using StackExchange.Redis;

namespace BookStoreApi.Service;

public class RedisCacheService : IRedisCacheService
{
    private readonly IConnectionMultiplexer _redis;

    public RedisCacheService(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    public async Task<T?> GetCacheValueAsync<T>(string key)
    {
        var db = _redis.GetDatabase();
        var json = await db.StringGetAsync(key);
        return json.HasValue
            ? JsonSerializer.Deserialize<T>(json.ToString())
            : default;
    }

    public async Task SetCacheValueAsync<T>(string key, T value, TimeSpan expiration)
    {
        var db = _redis.GetDatabase();
        var json = JsonSerializer.Serialize(value);
        await db.StringSetAsync(key, json, expiration);
    }

    public async Task<bool> RemoveDataAsync(string key)
    {
        var db = _redis.GetDatabase();
        bool isKeyExist = await db.KeyExistsAsync(key);
        if (isKeyExist == true)
        {
            return await db.KeyDeleteAsync(key);
        }
        return false;
    }
}
