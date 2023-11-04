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

        Command dayCommand = new("day")
        {
            dateArgument
        };
        dayCommand.AddAlias("d");
        dayCommand.SetHandler(LogDayAsync, dateArgument);

        Command weekCommand = new("week")
        {
            dateArgument
        };
        weekCommand.AddAlias("w");
        weekCommand.SetHandler(LogWeekAsync, dateArgument);

        Command monthCommand = new("month")
        {
            dateArgument
        };
        monthCommand.AddAlias("m");
        monthCommand.SetHandler(LogMonthAsync, dateArgument);

        Command cmd = new("log")
        {
            dayCommand,
            weekCommand,
            monthCommand,

            fromOption,
            toOption
        };
        cmd.SetHandler(LogAsync, fromOption, toOption);
        return cmd;
    }

    private static async Task LogDayAsync(DateOnly? date)
    {
        DateOnly theDay = date ?? DateOnly.FromDateTime(DateTime.Now);
        DateTime from = theDay.StartOfDay();
        DateTime to = theDay.EndOfDay();

        await LogAsync(from, to);
    }

    private static async Task LogWeekAsync(DateOnly? date)
    {
        DateOnly theDay = date ?? DateOnly.FromDateTime(DateTime.Now);
        DateTime from = theDay.StartOfWeek();
        DateTime to = theDay.EndOfWeek();

        await LogAsync(from, to);
    }

    private static async Task LogMonthAsync(DateOnly? date)
    {
        DateOnly theDay = date ?? DateOnly.FromDateTime(DateTime.Now);
        DateTime from = theDay.StartOfMonth();
        DateTime to = theDay.EndOfMonth();

        await LogAsync(from, to);
    }

    private static async Task LogAsync(DateOnly? from, DateOnly? to)
        => await LogAsync(from?.StartOfDay(), to?.EndOfDay());

    private static async Task LogAsync(DateTime? from, DateTime? to)
    {
        TimeTrackerDb db = await Helpers.GetDbAsync();
        LogDetails logDetails = await db.GetLogAsync(from, to);

        ConsoleUi.Log(from, to, logDetails);
    }
}
