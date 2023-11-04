using Xan.TimeTracker.Models;

namespace Xan.TimeTracker;

public class ConsoleUi
{
    public static void Error(string message)
    {
        ArgumentNullException.ThrowIfNull(message);

        Console.WriteLine(message);
    }

    public static void ListProjects(string[] projects)
    {
        ArgumentNullException.ThrowIfNull(projects);

        foreach (string project in projects)
        {
            Console.WriteLine(project);
        }
    }

    public static void Log(LogDetails details)
    {
        ArgumentNullException.ThrowIfNull(details);

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
                foreach (Duration duration in projectSummary.Details)
                {
                    Console.Write("    ");
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
        }
    }

    public static void StartedEntry(TimeEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        Console.WriteLine($"Started {entry.ProjectName} at {entry.Start}");
    }

    public static void Status(TimeEntry entry)
    {
        Console.Write($"{entry.ProjectName} started at {entry.Start}");
        if (entry.Comment is not null)
        {
            Console.Write($": {entry.Comment}");
        }
        Console.WriteLine();
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
}
