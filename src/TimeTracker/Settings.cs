namespace Xan.TimeTracker;

public class Settings
{
    public Settings()
    {
        DatabaseFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "TimeTracker.db");
    }

    public string DatabaseFilePath { get; set; }
}
