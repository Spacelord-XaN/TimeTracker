namespace Xan.TimeTracker.Models;

public record ProjectSummary(
    string Name,
    TimeSpan TotalDuration,
    IReadOnlyCollection<CommentSummary> Details
);
