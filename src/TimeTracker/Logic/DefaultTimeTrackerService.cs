using System.Drawing;
using Microsoft.EntityFrameworkCore;
using Xan.TimeTracker.Data;
using Xan.TimeTracker.Models;

namespace Xan.TimeTracker.Logic;

public class DefaultTimeTrackerService
    : ITimeTrackerService
{
    private readonly TimeTrackerDb _db;

    public DefaultTimeTrackerService(TimeTrackerDb db)
    {
        _db = db ?? throw new ArgumentNullException(nameof(db));
    }

    public async Task<IDictionary<DurationInfo, IReadOnlyCollection<TimeEntry>>> GetLogAsync(DateTime? from, DateTime? to)
    {
        IQueryable<TimeEntry> entries = _db.Entries
            .Where(entry => entry.End != null);

        if (from.HasValue)
        {
            entries = entries.Where(entry => entry.Start >= from.Value);
        }
        if (to.HasValue)
        {
            entries = entries.Where(entry => entry.End.Value <= to.Value);
        }

        IReadOnlyCollection<IGrouping<string, TimeEntry>> entriesByProject = await entries
            .GroupBy(entry => entry.ProjectName)
            .ToArrayAsync();

        Dictionary<DurationInfo, IReadOnlyCollection<TimeEntry>> result = new();

        foreach (IGrouping<string, TimeEntry> group in entriesByProject)
        {
            TimeSpan duration = group.Aggregate(TimeSpan.Zero, (duration, entry) => duration.Add(entry.GetDuration()));
            DurationInfo durationInfo = new(group.Key, duration);

            result.Add(durationInfo, group.ToArray());
        }

        return result;
    }

    public async Task<TimeEntry> GetRunningAsync()
    {
        return await _db.Entries.FirstAsync(entry => entry.End == null);
    }

    public async Task<bool> IsRunningAsync()
    {
        return await _db.Entries.AnyAsync(entry => entry.End == null);
    }

    public async Task<TimeEntry> StartAsync(DateTime timestamp, string projectName)
    {
        ArgumentNullException.ThrowIfNull(projectName);

        TimeEntry entry = new()
        {
            IsReviewed = false,
            ProjectName = projectName,
            Start = timestamp
        };

        _db.Entries.Add(entry);
        await _db.SaveChangesAsync();

        return entry;
    }

    public async Task<string[]> GetProjectsAsync()
    {
        return await _db.Entries
            .Select(entry => entry.ProjectName)
            .Distinct()
            .OrderBy(project => project)
            .ToArrayAsync();
    }

    public async Task<TimeEntry> StopAsync(DateTime timestamp)
    {
        TimeEntry entry = await GetRunningAsync();

        entry.End = timestamp;
        await _db.SaveChangesAsync();

        return entry;
    }
}
