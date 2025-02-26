using StackExchange.Redis;

namespace Valuator.Services;

public class RedisService : IRedisService
{
    private readonly ConnectionMultiplexer _redis;
    private readonly IDatabase _database;

    public RedisService(string connectionString)
    {
        _redis = ConnectionMultiplexer.Connect(connectionString);
        _database = _redis.GetDatabase();
    }
    public void SetString(string key, string value)
    {
        _database.StringSet(key, value);
    }

    public string? GetString(string key)
    {
        RedisValue value = _database.StringGet(key);
        return value.HasValue ? value.ToString() : null;
    }

    public List<string> GetAllKeys()
    {
        var server = _redis.GetServer(_redis.GetEndPoints().First());
        return server.Keys(pattern: "*").Select(k => k.ToString()).ToList();
    }
}
