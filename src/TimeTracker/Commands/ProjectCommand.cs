using System.CommandLine;
using Xan.TimeTracker.Data;

namespace Xan.TimeTracker.Commands;

public static class ProjectCommand
{
    public static Command Build()
    {
        Command listCommand = new("list");
        listCommand.AddAlias("l");
        listCommand.SetHandler(ListAsync);

        return new("proj")
        {
            listCommand
        };
    }

    private static async Task ListAsync()
    {
        TimeTrackerDb db = await Helpers.GetDbAsync();
        string[] projects = await db.GetProjectsAsync();

        ConsoleUi.ListProjects(projects);
    }
}
