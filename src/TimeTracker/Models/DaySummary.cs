namespace Xan.TimeTracker.Models;

public record DaySummary(
    DateOnly Date,
    IReadOnlyCollection<ProjectSummary> Projects
);
