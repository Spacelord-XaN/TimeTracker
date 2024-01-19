using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Spectre.Console;
using Xan.Extensions;
using Xan.TimeTracker.Data;

namespace Xan.TimeTracker;

public static class Helpers
{
    public static async Task<TimeTrackerDb> GetDbAsync()
    {
        IConfigurationRoot config = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .Build();
        Settings settings = config.Get<Settings>() ?? new Settings();

        TimeTrackerDb db = new(settings.DatabaseFilePath);

        if (!File.Exists(settings.DatabaseFilePath))
        {
            if (AnsiConsole.Confirm($"Database file ({settings.DatabaseFilePath}) does not exist, create new?"))
            {
                await db.Database.MigrateAsync();
            }
            else
            {
                Environment.Exit(-1);
            }
        }
        else
        {
            IEnumerable<string> pendingMigrations = await db.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
            {
                DateTime now = DateTime.Now;
                string backupFilePath = $"{settings.DatabaseFilePath}_{now:yyyy-MM-ddTHH-mm-ss}.bak";
                File.Copy(settings.DatabaseFilePath, backupFilePath, true);

                Console.WriteLine("Migrating database...");
                await db.Database.MigrateAsync();
                Console.WriteLine("Database migrated.");
            }
        }

        return db;
    }

    public static DateTime GetTimestamp(DateOnly? date, TimeOnly? time)
    {
        DateTime timestamp = DateTime.Now;
        if (date.HasValue)
        {
            timestamp = timestamp.Combine(date.Value);
        }
        if (time.HasValue)
        {
            timestamp = timestamp.Combine(time.Value);
        }
        return timestamp;
    }
}
