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

        DateOnly day = _clock.GetCurrentDate();
        if (verb.Date.HasValue)
        {
            day = verb.Date.Value;
        }

        IDictionary<DurationInfo, IReadOnlyCollection<TimeEntry>> entries = await _service.GetLogAsync(day);

        _ui.Log(entries);
    }
}
