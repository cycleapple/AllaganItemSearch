using System;
using System.Threading;
using System.Threading.Tasks;

using AllaganItemSearch.Mediator;
using Loc = AllaganItemSearch.Localization.Localization;

using DalaMock.Host.Mediator;

using Dalamud.Plugin;
using Dalamud.Plugin.Ipc;
using Dalamud.Plugin.Services;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AllaganItemSearch.Services;

public class ATService(ILogger<ATService> logger, MediatorService mediatorService, ICommandManager commandManager, IChatGui chatGui, IDalamudPluginInterface pluginInterface)
    : DisposableMediatorSubscriberBase(logger, mediatorService), IHostedService
{
    private readonly IChatGui chatGui = chatGui;
    private readonly IDalamudPluginInterface pluginInterface = pluginInterface;
    private ICallGateSubscriber<bool> isInitializedSubscriber;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        this.MediatorService.Subscribe<OpenMoreInformationMessage>(this, this.OpenMoreInformationSub);
        this.isInitializedSubscriber = this.pluginInterface.GetIpcSubscriber<bool>("AllaganTools.IsInitialized");

        return Task.CompletedTask;
    }

    public void OpenMoreInformationSub(OpenMoreInformationMessage openMoreInformation)
    {
        var isInitialized = false;
        try
        {
            isInitialized = this.isInitializedSubscriber.InvokeFunc();
        }
        catch (Exception e)
        {
        }

        if (isInitialized)
        {
            commandManager.ProcessCommand("/moreinfo " + openMoreInformation.ItemId);
        }
        else
        {
            this.chatGui.PrintError(Loc.T("Allagan Tools was not detected, please install it to get more information about this item."));
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
