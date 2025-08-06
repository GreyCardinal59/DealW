using DealW.Application.Services;
using DealW.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace DealW.API;

public static class DependencyInjection
{
    public static IServiceCollection AddDealWServices(this IServiceCollection services, IConfiguration configuration)
    {
        // ApplicationDbContext
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // Redis
        services.AddSingleton<ConnectionMultiplexer>(sp =>
        {
            var config = configuration.GetConnectionString("Redis") ?? "localhost";
            return ConnectionMultiplexer.Connect(config);
        });
        services.AddScoped<IRedisService, RedisService>();

        // Application Services
        services.AddScoped<UserService>();
        services.AddScoped<AuthService>();
        services.AddScoped<DuelService>();
        services.AddScoped<QuestionService>();
        services.AddScoped<QuestionCacheService>();

        // SignalR
        services.AddSignalR();

        // Background Services
        services.AddHostedService<MatchmakingBackgroundService>();

        return services;
    }
}