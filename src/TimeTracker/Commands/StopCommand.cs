using System.CommandLine;
using Xan.TimeTracker.Data;
using Xan.TimeTracker.Models;

namespace Xan.TimeTracker.Commands;

public static class StopCommand
{
    public static Command Build()
    {
        Option<DateOnly?> dateOption = new("--date");
        dateOption.AddAlias("-d");
        Option<TimeOnly?> timeOption = new("--time");
        timeOption.AddAlias("-t");

        Command cmd = new("stop")
        {
            dateOption,
            timeOption
        };
        cmd.SetHandler(StatusAsync, dateOption, timeOption);

        return cmd;
    }

    private static async Task StatusAsync(DateOnly? date, TimeOnly? time)
    {
        TimeTrackerDb db = await Helpers.GetDbAsync();
        if (await db.IsRunningAsync())
        {
            DateTime timestamp = Helpers.GetTimestamp(date, time);

            TimeEntry entry = await db.GetRunningAsync();

            entry.End = timestamp;
            await db.SaveChangesAsync();

            ConsoleUi.StoppedEntry(entry);
        }
    }
}
