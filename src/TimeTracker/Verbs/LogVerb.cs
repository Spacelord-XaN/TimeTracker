using CommandLine;

namespace Xan.TimeTracker.Verbs;

[Verb("log")]
public class LogVerb
{
    [Option('d', "day")]
    public DateOnly? Date { get; set; }
}
