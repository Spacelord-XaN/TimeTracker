namespace Xan.TimeTracker.Models;

#nullable disable warnings
public class TimeEntry
{
    public int Id { get; set; }

    public string ProjectName { get; set; }

    public DateTime Start { get; set; }

    public DateTime? End { get; set; }

    public bool IsReviewed { get; set; }

    public TimeSpan GetDuration()
        => End.Value - Start;
        
}
