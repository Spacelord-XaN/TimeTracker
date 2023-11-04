using System.CommandLine;
using Xan.TimeTracker.Data;
using Xan.TimeTracker.Models;

namespace Xan.TimeTracker.Commands;

public static class StatusCommand
{
    public static Command Build()
    {
        Command cmd = new("status");
        cmd.SetHandler(StatusAsync);

        return cmd;
    }

    private static async Task StatusAsync()
    {
        TimeTrackerDb db = await Helpers.GetDbAsync();
        if (await db.IsRunningAsync())
        {
            TimeEntry entry = await db.GetRunningAsync();
            ConsoleUi.Status(entry);
        }
    }
}
