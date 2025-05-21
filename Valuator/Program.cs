using Microsoft.AspNetCore.Authentication.Cookies;
using Services;
using Services.Database;

namespace Valuator;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddSingleton<IRedisService>(new RedisService());

        // Add services to the container.
        builder.Services.AddRazorPages();

        //builder.Services.AddSession(options =>
        //{
        //    options.IdleTimeout = TimeSpan.FromMinutes(30);
        //    options.Cookie.HttpOnly = true;
        //    options.Cookie.IsEssential = true;
        //    options.Cookie.Name = "Valuator.Session";
        //});

        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/Login";
                options.LogoutPath = "/Logout";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
            });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
        }
        app.UseStaticFiles();

        app.UseRouting();

        //app.UseAuthorization();
        app.UseAuthentication();   // добавление middleware аутентификации 
        app.UseAuthorization();   // добавление middleware авторизации 

        app.MapRazorPages();

        app.Run();
    }
}