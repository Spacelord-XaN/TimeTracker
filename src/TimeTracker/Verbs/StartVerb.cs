using CommandLine;

namespace Xan.TimeTracker.Verbs;

#nullable disable warnings
[Verb("start")]
public class StartVerb
{
    [Value(0, Required = true)]
    public string ProjectName { get; set; }
}
