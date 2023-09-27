using CommandLine;

namespace Xan.TimeTracker.Verbs;

[Verb("log")]
public class LogVerb
{
    [Option('d', "day", SetName = "dayLog")]
    public DateOnly? DayLogDate { get; set; }

    [Option('w', "week", SetName = "weekLog")]
    public  DateOnly? WeekLogDate { get; set; }

    [Option('m', "month", SetName = "monthLog")]
    public  DateOnly? MonthLogDate {get; set;}


    [Option('f', "from", SetName = "fromToLog")]
    public DateOnly? FromDate { get; set; }

    [Option('t', "to", SetName = "fromToLog")]
    public DateOnly? ToDate { get; set; }
}
