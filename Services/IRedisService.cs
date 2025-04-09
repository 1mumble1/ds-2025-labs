
namespace Services
{
    public interface IRedisService
    {
        List<string> GetAllKeys();
        string? GetString(string key);
        void SetString(string key, string value);
    }
}