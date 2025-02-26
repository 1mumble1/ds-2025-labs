namespace Valuator.Services;

public interface IRedisService
{
    string? GetString(string key);
    void SetString(string key, string value);
    List<string> GetAllKeys();
}