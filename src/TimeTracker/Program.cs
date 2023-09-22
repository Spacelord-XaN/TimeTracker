using CommandLine;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xan.Extensions;
using Xan.TimeTracker.Commands;
using Xan.TimeTracker.Data;
using Xan.TimeTracker.Logic;
using Xan.TimeTracker.Verbs;

namespace Xan.TimeTracker;

public static class Program
{
    public static async Task Main(string[] args)
    {
        Type[] verbs = new[]
        {
            typeof (LogVerb),
            typeof (StartVerb),
            typeof (StatusVerb),
            typeof (StopVerb)
        };

        await Parser.Default.ParseArguments(args, verbs)
              .WithParsedAsync(RunAsync);
    }

    private static async Task RunAsync(object verb)
    {
        if (verb is LogVerb logVerb)
        {
            await RunCommandAsync<LogCommand, LogVerb>(logVerb);
        }
        else if (verb is StartVerb startVerb)
        {
            await RunCommandAsync<StartCommand, StartVerb>(startVerb);
        }
        else if (verb is StatusVerb statusVerb)
        {
            await RunCommandAsync<StatusCommand, StatusVerb>(statusVerb);
        }
        else if (verb is StopVerb stopVerb)
        {
            await RunCommandAsync<StopCommand, StopVerb>(stopVerb);
        }
    }

    private static async Task RunCommandAsync<TCommand, TVerb>(TVerb verb)
        where TCommand : ICommand<TVerb>
    {
        ServiceCollection services = new ();
        ConfigureServices(services);

        IServiceProvider serviceProvider = services.BuildServiceProvider();
        await InitAppAsync(serviceProvider);
        
        using IServiceScope scope = serviceProvider.CreateScope();
        TCommand command = scope.ServiceProvider.GetRequiredService<TCommand>();
        await command.RunAsnc(verb);
    }

    public static async Task InitAppAsync(IServiceProvider serviceProvider)
    {
        using IServiceScope scope = serviceProvider.CreateScope();

        TimeTrackerDb db = scope.ServiceProvider.GetRequiredService<TimeTrackerDb>();
        await db.Database.EnsureCreatedAsync();
    }

    public static void ConfigureServices(IServiceCollection services)
    {
        string dbFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "TimeTracker.db");

        services.AddDbContext<TimeTrackerDb>(options =>
        {
            options.UseSqlite($"Data Source={dbFilePath}");
        });

        services
            .AddSingleton<IClock, SystemClock>()
            .AddScoped<ITimeTrackerService, DefaultTimeTrackerService>()
            .AddScoped<IUserInterface, ConsoleInterface>()
            .AddScoped<LogCommand>()
            .AddScoped<StartCommand>()
            .AddScoped<StatusCommand>()
            .AddScoped<StopCommand>()
            ;
    }
}
