using Xan.TimeTracker.Logic;
using Xan.TimeTracker.Verbs;

namespace Xan.TimeTracker.Commands;

public class ProjectCommand
    : ICommand<ProjectVerb>
{
    private readonly ITimeTrackerService _service;
    private readonly IUserInterface _ui;

    public ProjectCommand(ITimeTrackerService service, IUserInterface ui)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _ui = ui ?? throw new ArgumentNullException(nameof(ui));
    }

    public async Task RunAsnc(ProjectVerb verb)
    {
        ArgumentNullException.ThrowIfNull(verb);

        if (verb.List)
        {
            string[] projects = await _service.GetProjectsAsync();
            _ui.ListProjects(projects);
        }
        else
        {
            _ui.Error("Invalid options specified");
        }
    }
}
