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

    public async Task<LogDetails> GetLogAsync(DateTime? from, DateTime? to, string? projectName)
    {
        IQueryable<TimeEntry> entriesQuery = Entries;

        if (projectName is not null)
        {
            entriesQuery = entriesQuery.Where(entry => entry.ProjectName == projectName);
        }
        if (from.HasValue)
        {
            entriesQuery = entriesQuery.Where(entry => entry.Start >= from.Value);
        }
        if (to.HasValue)
        {
            entriesQuery = entriesQuery.Where(entry => (entry.End == null && entry.Start <= to) || (entry.End != null && entry.End.Value <= to.Value));
        }

        IReadOnlyCollection<TimeEntry> entries = await entriesQuery.ToArrayAsync();
        IEnumerable<IGrouping<DateTime, TimeEntry>> entriesByDay = entries.GroupBy(entry => entry.Start.Date);
        List<DaySummary> daySummaries = new();
        foreach (IGrouping<DateTime, TimeEntry> dayGroup in entriesByDay)
        {
            IEnumerable<IGrouping<string, TimeEntry>> groupByProject = dayGroup.GroupBy(entry => entry.ProjectName);

            List<ProjectSummary> projectSummaries = new();
            TimeSpan dayTotalDuration = TimeSpan.Zero;
            foreach (IGrouping<string, TimeEntry> projectGroup in groupByProject)
            {
                List<CommentSummary> commentSummaries = new();
                TimeSpan projectTotalDuration = TimeSpan.Zero;                

                IEnumerable<IGrouping<string?, TimeEntry>> commentGroups = projectGroup.GroupBy(entry => entry.Comment);
                foreach (IGrouping<string?, TimeEntry> commentGroup in commentGroups)
                {
                    List<Duration> commentDetails = new();
                    TimeSpan commentTotalDuration = TimeSpan.Zero;

                    foreach (TimeEntry entry in commentGroup.OrderBy(g => g.Start))
                    {
                        TimeOnly start = TimeOnly.FromDateTime(entry.Start);
                        TimeOnly? end = null;
                        if (entry.End.HasValue)
                        {
                            end = TimeOnly.FromDateTime(entry.End.Value);

                            TimeSpan timeSpan = end.Value - start;
                            commentTotalDuration += timeSpan;
                            projectTotalDuration += timeSpan;
                            dayTotalDuration += timeSpan;
                        }

                        Duration duration = new(start, end, entry.Comment);
                        commentDetails.Add(duration);
                    }

                    CommentSummary commentSummary = new(commentGroup.Key ?? string.Empty, commentTotalDuration, commentDetails);
                    commentSummaries.Add(commentSummary);
                }

                ProjectSummary projectSummary = new(projectGroup.Key, projectTotalDuration, commentSummaries);
                projectSummaries.Add(projectSummary);
            }

            DateOnly date = DateOnly.FromDateTime(dayGroup.Key);
            DaySummary daySummary = new(date, dayTotalDuration, projectSummaries);
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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.UseSqlite($"Data Source={_databaseFilePath}");
    }
}
