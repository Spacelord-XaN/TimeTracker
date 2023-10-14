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

    public void Log(IDictionary<DurationInfo, IReadOnlyCollection<TimeEntry>> entries)
    {
        ArgumentNullException.ThrowIfNull(entries);

        foreach (DurationInfo key in entries.Keys)
        {
            Console.WriteLine($"Project {key.ProjectName} took {key.Duration.Hours:00}:{key.Duration.Minutes:00} or {key.Duration.TotalHours:0.00} h");

            foreach (TimeEntry entry in entries[key])
            {
                Console.WriteLine($"  {entry.Start:d}: {entry.Start:t} - {entry.End.Value:t}");
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
}
