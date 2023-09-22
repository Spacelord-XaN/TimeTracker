using Microsoft.EntityFrameworkCore;
using Xan.TimeTracker.Models;

namespace Xan.TimeTracker.Data;

public class TimeTrackerDb
    : DbContext
{
    public TimeTrackerDb(DbContextOptions options)
        : base(options)
    {
    }

    public DbSet<TimeEntry> Entries { get; set; }
}
