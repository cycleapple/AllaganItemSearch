using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AllaganItemSearch.Filters;
using AllaganItemSearch.Windows;
using Loc = AllaganItemSearch.Localization.Localization;

using Dalamud.Game.Command;
using Dalamud.Plugin.Services;

using Microsoft.Extensions.Hosting;

namespace AllaganItemSearch.Services;

public class CommandService : IHostedService
{
    private readonly FilterState filterState;
    private readonly string[] commandName = { "/allaganitemsearch" , "/asearch"};
    private readonly StringFilter nameFilter;

    public CommandService(ICommandManager commandManager, MainWindow mainWindow, FilterService filterService, FilterState filterState)
    {
        this.filterState = filterState;
        this.CommandManager = commandManager;
        this.MainWindow = mainWindow;
        this.FilterService = filterService;
        this.nameFilter = (StringFilter)this.FilterService.Filters.First(c => c.Key == "name");
    }

    public ICommandManager CommandManager { get; }

    public MainWindow MainWindow { get; }

    public FilterService FilterService { get; }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        for (int i = 0; i < commandName.Length; i++)
        {
            this.CommandManager.AddHandler(
            commandName[i],
            new CommandInfo(this.OnCommand)
            {
                HelpMessage = Loc.T("Shows the Allagan Item Search main window. Text arguments after the command will be put into the name search field."),
            });
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        for (int i = 0; i < commandName.Length; i++)
        {
            this.CommandManager.RemoveHandler(commandName[i]);
        }

        return Task.CompletedTask;
    }

    private void OnCommand(string command, string arguments)
    {
        if (arguments.Length == 0)
        {
            this.MainWindow.Toggle();
        }
        else
        {
            this.MainWindow.IsOpen = true;
            this.nameFilter.UpdateFilterConfiguration(this.filterState, arguments);
        }
    }
}
