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
        Option<string?> commentOption = new("--comment");
        commentOption.AddAlias("-c");

        Command cmd = new("stop")
        {
            dateOption,
            timeOption,
            commentOption
        };
        cmd.SetHandler(StatusAsync, dateOption, timeOption, commentOption);

        return cmd;
    }

    private static async Task StatusAsync(DateOnly? date, TimeOnly? time, string? comment)
    {
        TimeTrackerDb db = await Helpers.GetDbAsync();
        if (await db.IsRunningAsync())
        {
            DateTime timestamp = Helpers.GetTimestamp(date, time);

            TimeEntry entry = await db.GetRunningAsync();

            entry.End = timestamp;
            if (comment is not null)
            {
                entry.Comment = comment;
            }
            await db.SaveChangesAsync();

            ConsoleUi.StoppedEntry(entry);
        }
    }
}
