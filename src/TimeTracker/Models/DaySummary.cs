namespace Xan.TimeTracker.Models;

public record DaySummary(
    DateOnly Date,
    TimeSpan TotalDuration,
    IReadOnlyCollection<ProjectSummary> Projects
);
