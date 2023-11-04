using Microsoft.EntityFrameworkCore;
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

        Argument<string> oldNameArgument = new("oldName");
        Argument<string> newNameArgument = new("newName");

        Command renameCommand = new("rename")
        {
            oldNameArgument,
            newNameArgument
        };
        renameCommand.AddAlias("r");
        renameCommand.SetHandler(RenameAsync, oldNameArgument, newNameArgument);

        return new("proj")
        {
            listCommand,
            renameCommand
        };
    }

    private static async Task ListAsync()
    {
        TimeTrackerDb db = await Helpers.GetDbAsync();
        string[] projects = await db.GetProjectsAsync();

        ConsoleUi.ListProjects(projects);
    }

    private static async Task RenameAsync(string oldName, string newName)
    {
        TimeTrackerDb db = await Helpers.GetDbAsync();

        await db.Entries
            .Where(entry => entry.ProjectName == oldName)
            .ExecuteUpdateAsync(setters => setters.SetProperty(entry => entry.ProjectName, newName));
    }
}
