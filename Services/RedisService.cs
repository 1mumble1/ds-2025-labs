using StackExchange.Redis;

namespace Services;

public class RedisService : IRedisService
{
    private ConnectionMultiplexer GetConnectionMultiplexer(string region)
    {
        string connectionString = Environment.GetEnvironmentVariable($"DB_{region.ToUpper()}") ?? throw new Exception("Failed to find env var");

        string password = Environment.GetEnvironmentVariable($"DB_{region.ToUpper()}_PASS") ?? throw new Exception("Failed to find env var");

        var configOptions = ConfigurationOptions.Parse(connectionString);
        configOptions.Password = password;

        return ConnectionMultiplexer.Connect(configOptions);
    }

    public void SetString(string region, string key, string value)
    {
        IDatabase redis = GetConnectionMultiplexer(region).GetDatabase();

        redis.StringSet(key, value);
    }

    public string? GetString(string region, string key)
    {
        Console.WriteLine($"LOOKUP: {key}, {region}");
        IDatabase redis = GetConnectionMultiplexer(region).GetDatabase();

        var value = redis.StringGet(key);
        return value.HasValue ? value.ToString() : null;
    }

    public List<string> GetAllKeys(string region)
    {
        var redis = GetConnectionMultiplexer(region);

        var server = redis.GetServer(redis.GetEndPoints().First());
        return server.Keys(pattern: "*").Select(k => k.ToString()).ToList();
    }
}
