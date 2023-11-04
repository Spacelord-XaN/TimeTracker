using System.CommandLine;
using Xan.Extensions;
using Xan.TimeTracker.Data;
using Xan.TimeTracker.Models;

namespace Xan.TimeTracker.Commands;

public static class LogCommand
{
    public static Command Build()
    {
        Option<DateOnly?> fromOption = new("--from");
        fromOption.AddAlias("-f");
        Option<DateOnly?> toOption = new("--to");
        toOption.AddAlias("-t");
        Argument<DateOnly?> dateArgument = new("date");
        Option<string?> projectNameOption = new("--project");
        projectNameOption.AddAlias("-p");

        Command dayCommand = new("day")
        {
            dateArgument,
            projectNameOption
        };
        dayCommand.AddAlias("d");
        dayCommand.SetHandler(LogDayAsync, dateArgument, projectNameOption);

        Command weekCommand = new("week")
        {
            dateArgument,
            projectNameOption
        };
        weekCommand.AddAlias("w");
        weekCommand.SetHandler(LogWeekAsync, dateArgument, projectNameOption);

        Command monthCommand = new("month")
        {
            dateArgument,
            projectNameOption
        };
        monthCommand.AddAlias("m");
        monthCommand.SetHandler(LogMonthAsync, dateArgument, projectNameOption);

        Command cmd = new("log")
        {
            dayCommand,
            weekCommand,
            monthCommand,

            fromOption,
            toOption,
            projectNameOption
        };
        cmd.SetHandler(LogAsync, fromOption, toOption, projectNameOption);
        return cmd;
    }

    private static async Task LogDayAsync(DateOnly? date, string? projectName)
    {
        DateOnly theDay = date ?? DateOnly.FromDateTime(DateTime.Now);
        DateTime from = theDay.StartOfDay();
        DateTime to = theDay.EndOfDay();

        await LogAsync(from, to, projectName);
    }

    private static async Task LogWeekAsync(DateOnly? date, string? projectName)
    {
        DateOnly theDay = date ?? DateOnly.FromDateTime(DateTime.Now);
        DateTime from = theDay.StartOfWeek();
        DateTime to = theDay.EndOfWeek();

        await LogAsync(from, to, projectName);
    }

    private static async Task LogMonthAsync(DateOnly? date, string? projectName)
    {
        DateOnly theDay = date ?? DateOnly.FromDateTime(DateTime.Now);
        DateTime from = theDay.StartOfMonth();
        DateTime to = theDay.EndOfMonth();

        await LogAsync(from, to, projectName);
    }

    private static async Task LogAsync(DateOnly? from, DateOnly? to, string? projectName)
        => await LogAsync(from?.StartOfDay(), to?.EndOfDay(), projectName);

    private static async Task LogAsync(DateTime? from, DateTime? to, string? projectName)
    {
        TimeTrackerDb db = await Helpers.GetDbAsync();
        LogDetails logDetails = await db.GetLogAsync(from, to, projectName);

        ConsoleUi.Log(from, to, logDetails);
    }
}
