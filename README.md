# TimeTracker
A small C# console application for simple time tracking

## Usage
```
tt start <projectName>
```
Starts tracking a new project with the given name

```
tt stop
```
Stops tracking the current project

```
tt status
```
If a project is tracking, it prints out the name and starting time stamp of the project
If no project is tracking, it prints out nothing

```
tt log <day>
```
Prints out a summary of all tracked projects for a specific day.


### Remarks
Currently only one project at a time can be tracked.

The data is stored in a SQLite database named TimeTrackder.db in the users profile directory.
SpecialFolder.UserProfile (https://learn.microsoft.com/en-us/dotnet/api/system.environment.specialfolder?view=net-7.0)

## Roadmap
- Extend log command
- Renaming of projects
- Improve output (colors, format)
