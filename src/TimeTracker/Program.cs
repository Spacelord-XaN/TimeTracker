using System.CommandLine;
using System.Diagnostics;
using Xan.TimeTracker.Commands;

namespace Xan.TimeTracker;

public static class Program
{
    public static async Task Main(string[] args)
    {
        Command rootCommand = new RootCommand
        {
            LogCommand.Build(),
            ProjectCommand.Build(),
            StartCommand.Build(),
            StatusCommand.Build(),
            StopCommand.Build(),
            ResumeCommand.Build()
        };
        await rootCommand.InvokeAsync(args);

        if (Debugger.IsAttached)
        {
            Console.ReadLine();
        }
    }
}
