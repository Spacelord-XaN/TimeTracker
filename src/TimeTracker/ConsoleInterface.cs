using Xan.TimeTracker.Models;

namespace Xan.TimeTracker;

public class ConsoleInterface
    : IUserInterface
{
    public void Error(string message)
    {
        ArgumentNullException.ThrowIfNull(message);

        Console.WriteLine(message);
    }

    public void ListProjects(string[] projects)
    {
        ArgumentNullException.ThrowIfNull(projects);

        foreach (string project in projects)
        {
            Console.WriteLine(project);
        }
    }

    public void Log(LogDetails details)
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

            Console.WriteLine(daySummary.Date);
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
                    Console.WriteLine();
                }
            }
        }
    }

    public void StartedEntry(TimeEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        Console.WriteLine($"Started {entry.ProjectName} at {entry.Start}");
    }

    public void Status(TimeEntry entry)
    {
        Console.WriteLine($"{entry.ProjectName} started at {entry.Start}");
    }

    public void StoppedEntry(TimeEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

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
