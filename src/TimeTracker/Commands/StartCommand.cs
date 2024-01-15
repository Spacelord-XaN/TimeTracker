using System.CommandLine;
using Microsoft.EntityFrameworkCore;
using Xan.TimeTracker.Data;
using Xan.TimeTracker.Models;

namespace Xan.TimeTracker.Commands;

public static class StartCommand
{
    public static Command Build()
    {
        Option<string?> projectNameOption = new("--project");
        projectNameOption.AddAlias("-p");
        Option<string?> commentOption = new("--comment");
        commentOption.AddAlias("-c");
        Option<DateOnly?> dateOption = new("--date");
        dateOption.AddAlias("-d");
        Option<TimeOnly?> timeOption = new("--time");
        timeOption.AddAlias("-t");

        Command cmd = new("start")
        {
            projectNameOption,
            commentOption,
            dateOption,
            timeOption
        };
        cmd.SetHandler(StartAsync, projectNameOption, dateOption, timeOption, commentOption);

        return cmd;
    }

    private static async Task StartAsync(string? projectName, DateOnly? date, TimeOnly? time, string? comment)
    {
        TimeTrackerDb db = await Helpers.GetDbAsync();
        if (await db.IsRunningAsync())
        {
            ConsoleUi.Error("Cannot start while running.");
            return;
        }

        string project;
        if (projectName is null)
        {
            string[] projectNames = await db.Entries.Select(entry => entry.ProjectName)
                .Distinct().OrderBy(name => name)
                .ToArrayAsync();
            project = ConsoleUi.PromptProjectName(projectNames);
        }
        else
        {
            project = projectName;
        }

        DateTime timestamp = Helpers.GetTimestamp(date, time);
        TimeEntry entry = new()
        {
            Comment = comment,
            ProjectName = project,
            Start = timestamp
        };

        db.Entries.Add(entry);
        await db.SaveChangesAsync();

        ConsoleUi.StartedEntry(entry);
    }
}
