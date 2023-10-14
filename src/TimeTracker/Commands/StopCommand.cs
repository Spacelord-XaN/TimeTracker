using Xan.Extensions;
using Xan.TimeTracker.Logic;
using Xan.TimeTracker.Models;
using Xan.TimeTracker.Verbs;

namespace Xan.TimeTracker.Commands;

public class StopCommand
    : ICommand<StopVerb>
{
    private readonly ITimeTrackerService _service;
    private readonly IUserInterface _ui;
    private readonly IClock _clock;

    public StopCommand(ITimeTrackerService service, IUserInterface ui, IClock clock)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _ui = ui ?? throw new ArgumentNullException(nameof(ui));
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
    }

    public async Task RunAsnc(StopVerb verb)
    {
        ArgumentNullException.ThrowIfNull(verb);

        if (await _service.IsRunningAsync())
        {
            DateTime stopTimestamp = _clock.GetCurrentDateTime();
            if (verb.Date.HasValue)
            {
                stopTimestamp = stopTimestamp.Combine(verb.Date.Value);
            }
            if (verb.Time.HasValue)
            {
                stopTimestamp = stopTimestamp.Combine(verb.Time.Value);
            }

            TimeEntry stoppedEntry = await _service.StopAsync(stopTimestamp);
            _ui.StoppedEntry(stoppedEntry);
        }
    }
}
