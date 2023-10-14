using Xan.TimeTracker.Models;

namespace Xan.TimeTracker.Logic;

public interface ITimeTrackerService
{
    Task<TimeEntry> StartAsync(DateTime timestamp, string projectName);

    Task<TimeEntry> StopAsync(DateTime timestamp);

    Task<bool> IsRunningAsync();

    Task<IDictionary<DurationInfo, IReadOnlyCollection<TimeEntry>>> GetLogAsync(DateTime? from, DateTime? to);

    Task<TimeEntry> GetRunningAsync();
}
