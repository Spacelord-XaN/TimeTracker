namespace Xan.TimeTracker.Models;

public record Duration(
    TimeOnly Start,
    TimeOnly? End
);
