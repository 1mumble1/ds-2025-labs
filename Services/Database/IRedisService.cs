namespace Services.Database;

public interface IRedisService
{
    List<string> GetAllKeys(string region);
    string? GetString(string region, string key);
    void SetString(string region, string key, string value);
}