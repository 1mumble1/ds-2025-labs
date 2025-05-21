using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services.Database;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Valuator.Pages;

public class RegisterModel : PageModel
{
    private readonly IRedisService _redisService;
    private readonly ILogger<RegisterModel> _logger;

    public RegisterModel(IRedisService redisService, ILogger<RegisterModel> logger)
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

        // Check if user exists
        string? existingUser = _redisService.GetString("users", $"USER-{username}");
        if (existingUser != null)
        {
            ErrorMessage = "Пользователь с таким логином уже существует";
            return Page();
        }

        // Hash password
        string passwordHash = HashPassword(password);

        // Store user
        _redisService.SetString("users", $"USER-{username}", passwordHash);

        // Automatically log in after registration
        ClaimsIdentity claimsIdentity = new(
            [
                new Claim(ClaimTypes.Name, username)
            ], CookieAuthenticationDefaults.AuthenticationScheme
        );

        ClaimsPrincipal principal = new ClaimsPrincipal(claimsIdentity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        return RedirectToPage("/Index");
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
}
