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

    public async Task<LogDetails> GetLogAsync(DateTime? from, DateTime? to)
    {
        IQueryable<TimeEntry> entriesQuery = _db.Entries;

        if (from.HasValue)
        {
            entriesQuery = entriesQuery.Where(entry => entry.Start >= from.Value);
        }
        if (to.HasValue)
        {
            entriesQuery = entriesQuery.Where(entry => entry.End == null || entry.End.Value <= to.Value);
        }

        IReadOnlyCollection<TimeEntry> entries = await entriesQuery.ToArrayAsync();
        IEnumerable<IGrouping<DateTime, TimeEntry>> entriesByDay = entries.GroupBy(entry => entry.Start.Date);
        List<DaySummary> daySummaries = new();
        foreach (IGrouping<DateTime, TimeEntry> dayGroup in entriesByDay)
        {
            IEnumerable<IGrouping<string, TimeEntry>> groupByProject = dayGroup.GroupBy(entry => entry.ProjectName);

            List<ProjectSummary> projectSummaries = new ();
            foreach (IGrouping<string, TimeEntry> projectGroup in groupByProject)
            {
                List<Duration> projectDetails = new ();
                TimeSpan projectTotalDuration = TimeSpan.Zero;
                foreach (TimeEntry entry in projectGroup)
                {
                    TimeOnly start = TimeOnly.FromDateTime(entry.Start);
                    TimeOnly? end = null;
                    if (entry.End.HasValue)
                    {
                        end = TimeOnly.FromDateTime(entry.End.Value);
                        projectTotalDuration = projectTotalDuration.Add(end.Value - start);
                    }
                    Duration duration = new (start, end);
                    projectDetails.Add(duration);
                }

                ProjectSummary projectSummary = new (projectGroup.Key, projectTotalDuration, projectDetails);
                projectSummaries.Add(projectSummary);
            }

            DateOnly date = DateOnly.FromDateTime(dayGroup.Key);
            DaySummary daySummary = new (date, projectSummaries);
            daySummaries.Add(daySummary);
        }

        return new LogDetails(daySummaries);
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
