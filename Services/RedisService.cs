using StackExchange.Redis;

namespace Services;

public class RedisService : IRedisService
{
    //private readonly ConnectionMultiplexer _redis;
    //private readonly IDatabase _database;

    //public RedisService(string connectionString)
    //{
    //    _redis = ConnectionMultiplexer.Connect(connectionString);
    //    _database = _redis.GetDatabase();
    //}
    public void SetString(string region, string key, string value)
    {
        //_database.StringSet(key, value);
        string? connectionString = Environment.GetEnvironmentVariable($"DB_{region.ToUpper()}") ?? throw new Exception("Failed to find env var");
        IDatabase redis = ConnectionMultiplexer.Connect(connectionString).GetDatabase();

        redis.StringSet(key, value);
    }

    public string? GetString(string region, string key)
    {
        //RedisValue value = _database.StringGet(key);
        //return value.HasValue ? value.ToString() : null;
        //return null;

        string? connectionString = Environment.GetEnvironmentVariable($"DB_{region.ToUpper()}") ?? throw new Exception("Failed to find env var");
        IDatabase redis = ConnectionMultiplexer.Connect(connectionString).GetDatabase();

        var value = redis.StringGet(key);
        return value.HasValue ? value.ToString() : null;
    }

    public List<string> GetAllKeys(string region)
    {
        //var server = _redis.GetServer(_redis.GetEndPoints().First());
        //return server.Keys(pattern: "*").Select(k => k.ToString()).ToList();
        //return new List<string>();

        string? connectionString = Environment.GetEnvironmentVariable($"DB_{region.ToUpper()}") ?? throw new Exception("Failed to find env var");
        var redis = ConnectionMultiplexer.Connect(connectionString);

        var server = redis.GetServer(redis.GetEndPoints().First());
        return server.Keys(pattern: "*").Select(k => k.ToString()).ToList();
    }
}
