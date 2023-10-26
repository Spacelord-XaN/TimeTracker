using Xan.TimeTracker.Models;

namespace Xan.TimeTracker;

public interface IUserInterface
{
    void Error(string message);

    void ListProjects(string[] projects);

    void Log(LogDetails details);

    void StartedEntry(TimeEntry entry);

    void Status(TimeEntry entry);

    void StoppedEntry(TimeEntry entry);
}
