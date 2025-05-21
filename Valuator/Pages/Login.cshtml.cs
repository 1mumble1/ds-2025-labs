using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Database;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Valuator.Pages;

public class LoginModel : PageModel
{
    private readonly IRedisService _redisService;
    private readonly ILogger<LoginModel> _logger;

    public LoginModel(IRedisService redisService, ILogger<LoginModel> logger)
    {
        _redisService = redisService;
        _logger = logger;
    }

    public string ErrorMessage { get; set; } = string.Empty;

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync(string username, string password)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            ErrorMessage = "Логин и пароль обязательны";
            return Page();
        }

        // Get user from Redis
        string? storedHash = _redisService.GetString("users", $"USER-{username}");
        if (storedHash == null)
        {
            ErrorMessage = "Неверный логин или пароль";
            return Page();
        }

        // Verify password
        string inputHash = HashPassword(password);
        if (inputHash != storedHash)
        {
            ErrorMessage = "Неверный логин или пароль";
            return Page();
        }

        // Login successful
        ClaimsIdentity claimsIdentity = new(
    [
        new Claim(ClaimTypes.Name, username)
    ], CookieAuthenticationDefaults.AuthenticationScheme
);

        ClaimsPrincipal principal = new ClaimsPrincipal(claimsIdentity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        return Redirect("/Index");
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
}
