using Microsoft.EntityFrameworkCore;
using System.CommandLine;
using Xan.TimeTracker.Data;
using Xan.TimeTracker.Models;

namespace Xan.TimeTracker.Commands;

public static class ProjectCommand
{
    public static Command Build()
    {
        Option<bool> commentsOption = new("--comments");
        commentsOption.AddAlias("-c");
        Option<string?> projectNameOption = new("--project");
        projectNameOption.AddAlias("-p");

        Command listCommand = new("list")
        {
            commentsOption,
            projectNameOption
        };
        listCommand.AddAlias("l");
        listCommand.SetHandler(ListAsync, commentsOption, projectNameOption);

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

    private static async Task ListAsync(bool includeComments, string? projectName)
    {
        TimeTrackerDb db = await Helpers.GetDbAsync();

        IQueryable<TimeEntry> entries = db.Entries;
        if (projectName is not null)
        {
            entries = entries.Where(entry => entry.ProjectName == projectName);
        }

        List<ListProjectsModel> result;
        if (includeComments)
        {
            result = new List<ListProjectsModel>();

            TimeEntry[] allEntries = await entries.ToArrayAsync();
            foreach (IGrouping<string, TimeEntry> projectGroup in allEntries.GroupBy(entry => entry.ProjectName))
            {
                IEnumerable<string> comments = projectGroup.Select(entry => entry.Comment ?? string.Empty).Distinct().Order();
                ListProjectsModel project = new(projectGroup.Key, comments.ToArray());

                result.Add(project);
            }
        }
        else
        {
            string[] projects = await entries
                .Select(entry => entry.ProjectName)
                .Distinct()
                .OrderBy(project => project)
                .ToArrayAsync();

            result = new List<ListProjectsModel>(projects.Select(name => new ListProjectsModel(name, Array.Empty<string>())));
        }

        ConsoleUi.ListProjects(result);
    }

    private static async Task RenameAsync(string oldName, string newName)
    {
        TimeTrackerDb db = await Helpers.GetDbAsync();

        await db.Entries
            .Where(entry => entry.ProjectName == oldName)
            .ExecuteUpdateAsync(setters => setters.SetProperty(entry => entry.ProjectName, newName));
    }
}
