using System.Text;
using System.Text.Json;
using APBD1.Data;
using APBD1.Data.Models;
using APBD1.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace APBD1;

class Program
{
    static async Task Main(string[] args)
    {
        AppOptions appOpts;
        try
        {
            await using var appOptsStream = File.OpenRead("appsettings.json");
            appOpts = await JsonSerializer.DeserializeAsync<AppOptions>(appOptsStream)
                ?? throw new InvalidOperationException("Failed to deserialize appsettings.json");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to load configuration: {e.Message}");
            return;
        }

        var services = new ServiceCollection();
        services.AddSingleton(appOpts);
        services.AddDbContext<AppDbContext>(
            o => o.UseSqlite("Data Source=app-prod.db"));
        services.AddSingleton<UserRepository>();
        services.AddSingleton<DeviceRepository>();
        services.AddSingleton<LeaseRepository>();

        await using var provider = services.BuildServiceProvider();

        await using (var scope = provider.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await db.Database.MigrateAsync();
        }

        var cmd = new CommandPalette(
            provider.GetRequiredService<UserRepository>(),
            provider.GetRequiredService<DeviceRepository>(),
            provider.GetRequiredService<LeaseRepository>()
        );

        await cmd.RunReplAsync(appOpts.ExitCommands);
    }
}
