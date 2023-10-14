# TimeTracker
A small C# console application for simple time tracking

## Usage
```
tt start <projectName> -d YYYY-MM-DD -t hh:mm[:ss]
```
Starts tracking a new project with the given name and the specified date and time.
\
The -d and -t parameters are optional.

```
tt stop -d YYYY-MM-DD -t hh:mm[:ss]
```
Stops tracking the current project with the specified date and time.
\
The -d and -t parameters are optional.

```
tt status
```
If a project is tracking, it prints out the name and starting time stamp of the project
If no project is tracking, it prints out nothing

```
tt log -d YYYY-MM-DD
tt log -w YYYY-MM-DD
tt log -m YYYY-MM-DD
tt log --from YYYY-MM-DD
tt log --to YYYY-MM-DD
tr log --from YYYY-MM-DD --to YYYY-MM-DD
```
Prints out a summary of all tracked projects for the specified time span:
- d Day
- w Week
- m Month 

```
tt proj -l
```
Lists all projects.

### Remarks
Currently only one project at a time can be tracked.

The data is stored in a SQLite database named TimeTrackder.db in the users profile directory.
SpecialFolder.UserProfile (https://learn.microsoft.com/en-us/dotnet/api/system.environment.specialfolder?view=net-7.0)

## Roadmap
- Renaming of projects
- Improve output (colors, format)
