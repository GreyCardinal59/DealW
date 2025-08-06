using DealW.Infrastructure;
using DealW.Infrastructure.Seeders;
using Microsoft.EntityFrameworkCore;

namespace DealW.API;

public static class SeederExtensions
{
    public static WebApplication ApplyMigrations(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        
        try
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            
            context.Database.Migrate();
            
            var seeder = new Seeder(context);
            seeder.Seed();
            
            app.Logger.LogInformation("Database migrations applied and data seeded successfully.");
        }
        catch (Exception ex)
        {
            app.Logger.LogError(ex, "An error occurred while applying migrations or seeding the database.");
        }
        
        return app;
    }
}