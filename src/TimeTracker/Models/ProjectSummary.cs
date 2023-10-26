namespace Xan.TimeTracker.Models;

public record ProjectSummary(
    string Name,
    TimeSpan TotalDuration,
    IReadOnlyCollection<Duration> Details
);
