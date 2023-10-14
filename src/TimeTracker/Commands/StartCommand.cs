using Xan.Extensions;
using Xan.TimeTracker.Logic;
using Xan.TimeTracker.Models;
using Xan.TimeTracker.Verbs;

namespace Xan.TimeTracker.Commands;

public class StartCommand
    : ICommand<StartVerb>
{
    private readonly ITimeTrackerService _service;
    private readonly IUserInterface _ui;
    private readonly IClock _clock;

    public StartCommand(ITimeTrackerService service, IUserInterface ui, IClock clock)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _ui = ui ?? throw new ArgumentNullException(nameof(ui));
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
    }

    public async Task RunAsnc(StartVerb verb)
    {
        ArgumentNullException.ThrowIfNull(verb);

        if (await _service.IsRunningAsync())
        {
            _ui.Error("Cannot start while running.");
            return;
        }

        DateTime startTimestamp = _clock.GetCurrentDateTime();
        if (verb.Date.HasValue)
        {
            startTimestamp = startTimestamp.Combine(verb.Date.Value);
        }
        if (verb.Time.HasValue)
        {
            startTimestamp = startTimestamp.Combine(verb.Time.Value);
        }

        TimeEntry startedEntry = await _service.StartAsync(startTimestamp, verb.ProjectName);
        _ui.StartedEntry(startedEntry);
    }
}
