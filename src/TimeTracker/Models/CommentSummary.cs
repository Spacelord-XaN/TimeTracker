namespace Xan.TimeTracker.Models;

public record CommentSummary(
    string Comment,
    TimeSpan TotalDuration,
    IReadOnlyCollection<Duration> Details
);
