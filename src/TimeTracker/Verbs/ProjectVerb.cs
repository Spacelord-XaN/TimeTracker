using CommandLine;

namespace Xan.TimeTracker.Verbs;

#nullable disable warnings
[Verb("proj")]
public class ProjectVerb
{
    [Option('l', "list", SetName = "list")]
    public bool List { get; set; }
}
