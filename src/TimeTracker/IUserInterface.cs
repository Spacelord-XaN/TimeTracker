﻿using Xan.TimeTracker.Models;

namespace Xan.TimeTracker;

public interface IUserInterface
{
    void Error(string message);

    void Log(IDictionary<DurationInfo, IReadOnlyCollection<TimeEntry>> entries);

    void StartedEntry(TimeEntry entry);

    void Status(TimeEntry entry);

    void StoppedEntry(TimeEntry entry);
}
