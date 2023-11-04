using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
        await db.Database.MigrateAsync();
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
