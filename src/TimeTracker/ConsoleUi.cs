using Spectre.Console;
using Xan.TimeTracker.Models;

namespace Xan.TimeTracker;

public class ConsoleUi
{
    public static void Error(string message)
    {
        ArgumentNullException.ThrowIfNull(message);

        Console.WriteLine(message);
    }

    public static void ListProjects(IReadOnlyCollection<ListProjectsModel> projects)
    {
        ArgumentNullException.ThrowIfNull(projects);

        foreach (ListProjectsModel project in projects)
        {
            Console.WriteLine(project.Name);
            foreach (string comment in project.Comments)
            {
                Console.WriteLine($"  {comment}");
            }
        }
    }

    public static void Log(DateTime? from, DateTime? to, LogDetails details)
    {
        ArgumentNullException.ThrowIfNull(details);

        if (from.HasValue || to.HasValue)
        {
            Console.Write("Range:");
            if (from.HasValue)
            {
                Console.Write($" from {from.Value:f}");
            }
            if (to.HasValue)
            {
                Console.Write($" to {to.Value:f}");
            }
            Console.WriteLine();
            Console.WriteLine();
        }

        bool firstDay = true;
        foreach (DaySummary daySummary in details.DaySummaries)
        {
            if (firstDay)
            {
                firstDay = false;
            }
            else
            {
                Console.WriteLine();
            }

            Console.WriteLine($"{daySummary.Date} => {daySummary.TotalDuration.Hours:00}:{daySummary.TotalDuration.Minutes:00} or {daySummary.TotalDuration.TotalHours:0.00} h");
            foreach (ProjectSummary projectSummary in daySummary.Projects)
            {
                Console.WriteLine($"  {projectSummary.Name} => {projectSummary.TotalDuration.Hours:00}:{projectSummary.TotalDuration.Minutes:00} or {projectSummary.TotalDuration.TotalHours:0.00} h");
                foreach (CommentSummary commentSummary in projectSummary.Details)
                {
                    string commentPart = "";
                    if (!string.IsNullOrEmpty(commentSummary.Comment))
                    {
                        commentPart = $"{commentSummary.Comment} => ";
                    }
                    Console.WriteLine($"    {commentPart}{commentSummary.TotalDuration.Hours:00}:{commentSummary.TotalDuration.Minutes:00} or {commentSummary.TotalDuration.TotalHours:0.00} h");
                    foreach (Duration duration in commentSummary.Details)
                    {
                        Console.Write("      ");
                        if (duration.End.HasValue)
                        {
                            Write($"{duration.Start} - {duration.End.Value}", ConsoleColor.Green);
                        }
                        else
                        {
                            Write($"{duration.Start}", ConsoleColor.Red);
                        }

                        if (duration.Comment is not null)
                        {
                            Console.Write($": {duration.Comment}");
                        }
                        Console.WriteLine();
                    }
                }
                Console.WriteLine();
            }
            WriteSingleCharLine('-');
        }
    }

    public static string PromptProjectName(string[] projectNames)
    {
        ArgumentNullException.ThrowIfNull(projectNames);

        return AnsiConsole.Prompt(
            new SelectionPrompt<string>()
            .Title("Select project")
            .AddChoices(projectNames));
    }

    public static void StartedEntry(TimeEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        Console.WriteLine($"Started {entry.ProjectName} at {entry.Start}");
    }

    public static void Status(TimeEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        Console.WriteLine(entry.ProjectName);
        Console.WriteLine($"Start: {entry.Start}");
        if (entry.Comment is not null)
        {
            Console.WriteLine($" {entry.Comment}");
        }
    }

    public static void StoppedEntry(TimeEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);
        ArgumentNullException.ThrowIfNull(entry.End);

        Console.WriteLine($"Stopped {entry.ProjectName}: {entry.Start} until {entry.End.Value}");
    }

    private static void Write(string message, ConsoleColor color)
    {
        ConsoleColor initialColor = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.Write(message);
        Console.ForegroundColor = initialColor;
    }
    
    private static void WriteSingleCharLine(char c)
    {
        string line = new string(Enumerable.Range(0, Console.BufferWidth).Select(_ => c).ToArray());
        Console.WriteLine(line);
    }
}
