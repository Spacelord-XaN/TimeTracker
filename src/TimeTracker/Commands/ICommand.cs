namespace Xan.TimeTracker.Commands;

public interface ICommand<TVerb>
{
    Task RunAsnc(TVerb verb);
}
