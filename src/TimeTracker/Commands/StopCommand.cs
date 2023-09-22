using Xan.TimeTracker.Logic;
using Xan.TimeTracker.Models;
using Xan.TimeTracker.Verbs;

namespace Xan.TimeTracker.Commands;

public class StopCommand
    : ICommand<StopVerb>
{
    private readonly ITimeTrackerService _service;
    private readonly IUserInterface _ui;

    public StopCommand(ITimeTrackerService service, IUserInterface ui)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _ui = ui ?? throw new ArgumentNullException(nameof(ui));
    }

    public async Task RunAsnc(StopVerb verb)
    {
        ArgumentNullException.ThrowIfNull(verb);

        if (await _service.IsRunningAsync())
        {
            TimeEntry stoppedEntry = await _service.StopAsync();
            _ui.StoppedEntry(stoppedEntry);
        }
    }
}
