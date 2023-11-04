namespace Xan.TimeTracker.Models;

public record ListProjectsModel(
    string Name,
    IReadOnlyCollection<string> Comments
);
