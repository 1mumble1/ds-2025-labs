using Services.Database;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Services.Models;

public class UserService
{
    private readonly IRedisService _redisService;

    public UserService(IRedisService redisService)
    {
        _redisService = redisService;
    }

    public User CreateUser(string username, string password)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            throw new ArgumentException("Username and password are required");
        }

        // Check if user already exists
        if (_redisService.GetString("main", $"USERNAME-{username}") != null)
        {
            var existingUserStr = _redisService.GetString("main", $"USERNAME-{username}");
            var existingUser = JsonSerializer.Deserialize<User>(existingUserStr);
            return existingUser;
        }

        var user = new User
        {
            Username = username,
            PasswordHash = HashPassword(password)
        };

        _redisService.SetString("main", $"USERNAME-{username}", JsonSerializer.Serialize(user));

        return user;
    }

    public User? Authenticate(string username, string password)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            return null;
        }

        var userId = _redisService.GetString("users", $"username:{username}");
        if (userId == null)
        {
            return null;
        }

        var storedPasswordHash = _redisService.GetString("users", $"user:{userId}:password");
        if (storedPasswordHash == null)
        {
            return null;
        }

        if (!VerifyPassword(password, storedPasswordHash))
        {
            return null;
        }

        return new User
        {
            Username = username,
            PasswordHash = storedPasswordHash
        };
    }

    public User? GetById(string id)
    {
        var username = _redisService.GetString("users", $"id:{id}");
        if (username == null) return null;

        return new User
        {
            Username = username,
            PasswordHash = _redisService.GetString("users", $"user:{id}:password") ?? string.Empty
        };
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    private static bool VerifyPassword(string password, string storedHash)
    {
        return HashPassword(password) == storedHash;
    }

}
