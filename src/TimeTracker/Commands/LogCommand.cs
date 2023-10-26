using Xan.Extensions;
using Xan.TimeTracker.Logic;
using Xan.TimeTracker.Models;
using Xan.TimeTracker.Verbs;

namespace Xan.TimeTracker.Commands;

public class LogCommand
    : ICommand<LogVerb>
{
    private readonly ITimeTrackerService _service;
    private readonly IUserInterface _ui;
    private readonly IClock _clock;

    public LogCommand(ITimeTrackerService service, IUserInterface ui, IClock clock)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _ui = ui ?? throw new ArgumentNullException(nameof(ui));
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
    }

    public async Task RunAsnc(LogVerb verb)
    {
        ArgumentNullException.ThrowIfNull(verb);

        DateOnly currentDate = _clock.GetCurrentDate();
        DateTime? from = null;
        DateTime? to = null;
        if (verb.DayLogDate.HasValue)
        {
            from = verb.DayLogDate.Value.StartOfDay();
            to = verb.DayLogDate.Value.EndOfDay();
        }
        else if (verb.WeekLogDate.HasValue)
        {
            from = verb.WeekLogDate.Value.StartOfWeek();
            to = verb.WeekLogDate.Value.EndOfWeek();
        }
        else if (verb.MonthLogDate.HasValue)
        {
            from = verb.MonthLogDate.Value.StartOfMonth();
            to = verb.MonthLogDate.Value.EndOfMonth();
        }
        else if (verb.FromDate.HasValue || verb.ToDate.HasValue)
        {
            if (verb.FromDate.HasValue)
            {
                from = verb.FromDate.Value.StartOfDay();
            }
            if (verb.ToDate.HasValue)
            {
                to = verb.ToDate.Value.EndOfDay();
            }
        }
        else
        {
            from = currentDate.StartOfDay();
            to = currentDate.EndOfDay();
        }

        LogDetails logDetails = await _service.GetLogAsync(from, to);

        _ui.Log(logDetails);
    }
}
