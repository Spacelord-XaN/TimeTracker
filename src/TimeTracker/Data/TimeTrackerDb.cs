using Microsoft.EntityFrameworkCore;
using Xan.TimeTracker.Models;

namespace Xan.TimeTracker.Data;

public class TimeTrackerDb
    : DbContext
{
    private readonly string _databaseFilePath;

    public TimeTrackerDb()
    {
        _databaseFilePath = ":memory:";
    }

    public TimeTrackerDb(string databaseFilePath)
    {
        _databaseFilePath = databaseFilePath ?? throw new ArgumentNullException(nameof(databaseFilePath));
    }

    public DbSet<TimeEntry> Entries { get; set; }

    public async Task<LogDetails> GetLogAsync(DateTime? from, DateTime? to)
    {
        IQueryable<TimeEntry> entriesQuery = Entries;

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

            List<ProjectSummary> projectSummaries = new();
            foreach (IGrouping<string, TimeEntry> projectGroup in groupByProject)
            {
                List<Duration> projectDetails = new();
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
                    Duration duration = new(start, end);
                    projectDetails.Add(duration);
                }

                ProjectSummary projectSummary = new(projectGroup.Key, projectTotalDuration, projectDetails);
                projectSummaries.Add(projectSummary);
            }

            DateOnly date = DateOnly.FromDateTime(dayGroup.Key);
            DaySummary daySummary = new(date, projectSummaries);
            daySummaries.Add(daySummary);
        }

        return new LogDetails(daySummaries);
    }

    public async Task<TimeEntry> GetRunningAsync()
    {
        return await Entries.FirstAsync(entry => entry.End == null);
    }

    public async Task<bool> IsRunningAsync()
    {
        return await Entries.AnyAsync(entry => entry.End == null);
    }

    public async Task<string[]> GetProjectsAsync()
    {
        return await Entries
            .Select(entry => entry.ProjectName)
            .Distinct()
            .OrderBy(project => project)
            .ToArrayAsync();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.UseSqlite($"Data Source={_databaseFilePath}");
    }
}
