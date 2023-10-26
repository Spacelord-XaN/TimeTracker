namespace Xan.TimeTracker.Models;

public record LogDetails(
    IReadOnlyCollection<DaySummary> DaySummaries
);
