using Xan.TimeTracker.Logic;
using Xan.TimeTracker.Models;
using Xan.TimeTracker.Verbs;

namespace Xan.TimeTracker.Commands;

public class StatusCommand
    : ICommand<StatusVerb>
{
    private readonly ITimeTrackerService _service;
    private readonly IUserInterface _ui;

    public StatusCommand(ITimeTrackerService service, IUserInterface ui)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _ui = ui ?? throw new ArgumentNullException(nameof(ui));
    }

    public async Task RunAsnc(StatusVerb verb)
    {
        ArgumentNullException.ThrowIfNull(verb);

        if (await _service.IsRunningAsync())
        {
            TimeEntry entry = await _service.GetRunningAsync();
            _ui.Status(entry);
        }
    }
}
