using System.CommandLine;
using Microsoft.EntityFrameworkCore;
using Xan.TimeTracker.Data;
using Xan.TimeTracker.Models;

namespace Xan.TimeTracker.Commands;

public static class ResumeCommand
{
    public static Command Build()
    {
        Option<DateOnly?> dateOption = new("--date");
        dateOption.AddAlias("-d");
        Option<TimeOnly?> timeOption = new("--time");
        timeOption.AddAlias("-t");

        Command cmd = new("resume")
        {
            dateOption,
            timeOption
        };
        cmd.SetHandler(ResumeAsync, dateOption, timeOption);

        return cmd;
    }

    private static async Task ResumeAsync(DateOnly? date, TimeOnly? time)
    {
        TimeTrackerDb db = await Helpers.GetDbAsync();
        if (await db.IsRunningAsync())
        {
            ConsoleUi.Error("Cannot resume while running.");
            return;
        }

        TimeEntry? lastEntry = await db.Entries.OrderByDescending(entry => entry.End).FirstOrDefaultAsync();
        if (lastEntry == null)
        {
            ConsoleUi.Error("Not entry found for resuming.");
            return;
        }

        DateTime timestamp = Helpers.GetTimestamp(date, time);
        TimeEntry entry = new()
        {
            Comment = lastEntry.Comment,
            ProjectName = lastEntry.ProjectName,
            Start = timestamp
        };

        db.Entries.Add(entry);
        await db.SaveChangesAsync();

        ConsoleUi.StartedEntry(entry);
    }
}
