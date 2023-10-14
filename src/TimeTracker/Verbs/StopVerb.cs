using CommandLine;

namespace Xan.TimeTracker.Verbs;

[Verb("stop")]
public class StopVerb
{
    [Option('d', "date")]
    public DateOnly? Date { get; set; }

    [Option('t', "time")]
    public  TimeOnly? Time { get; set; }
}
