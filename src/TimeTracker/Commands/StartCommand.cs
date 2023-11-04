using System.CommandLine;
using Xan.TimeTracker.Data;
using Xan.TimeTracker.Models;

namespace Xan.TimeTracker.Commands;

public static class StartCommand
{
    public static Command Build()
    {
        Argument<string> projectNameArgument = new("projectName");
        Option<DateOnly?> dateOption = new("--date");
        dateOption.AddAlias("-d");
        Option<TimeOnly?> timeOption = new("--time");
        timeOption.AddAlias("-t");
        Option<string?> commentOption = new("--comment");
        commentOption.AddAlias("-c");

        Command cmd = new("start")
        {
            projectNameArgument,
            dateOption,
            timeOption,
            commentOption
        };
        cmd.SetHandler(StartAsync, projectNameArgument, dateOption, timeOption, commentOption);

        return cmd;
    }

    private static async Task StartAsync(string projectName, DateOnly? date, TimeOnly? time, string? comment)
    {
        ArgumentNullException.ThrowIfNull(projectName);

        TimeTrackerDb db = await Helpers.GetDbAsync();
        if (await db.IsRunningAsync())
        {
            ConsoleUi.Error("Cannot start while running.");
            return;
        }

        DateTime timestamp = Helpers.GetTimestamp(date, time);
        TimeEntry entry = new()
        {
            Comment = comment,
            ProjectName = projectName,
            Start = timestamp
        };

        db.Entries.Add(entry);
        await db.SaveChangesAsync();

        ConsoleUi.StartedEntry(entry);
    }
}
