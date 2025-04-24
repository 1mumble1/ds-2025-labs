using StackExchange.Redis;

namespace Services;

public class RedisService : IRedisService
{
    public void SetString(string region, string key, string value)
    {
        string? connectionString = Environment.GetEnvironmentVariable($"DB_{region.ToUpper()}") ?? throw new Exception("Failed to find env var");
        IDatabase redis = ConnectionMultiplexer.Connect(connectionString).GetDatabase();

        redis.StringSet(key, value);
    }

    public string? GetString(string region, string key)
    {
        Console.WriteLine($"LOOKUP: {key}, {region}");
        string? connectionString = Environment.GetEnvironmentVariable($"DB_{region.ToUpper()}") ?? throw new Exception("Failed to find env var");
        IDatabase redis = ConnectionMultiplexer.Connect(connectionString).GetDatabase();

        var value = redis.StringGet(key);
        return value.HasValue ? value.ToString() : null;
    }

    public List<string> GetAllKeys(string region)
    {
        string? connectionString = Environment.GetEnvironmentVariable($"DB_{region.ToUpper()}") ?? throw new Exception("Failed to find env var");
        var redis = ConnectionMultiplexer.Connect(connectionString);

        var server = redis.GetServer(redis.GetEndPoints().First());
        return server.Keys(pattern: "*").Select(k => k.ToString()).ToList();
    }
}
