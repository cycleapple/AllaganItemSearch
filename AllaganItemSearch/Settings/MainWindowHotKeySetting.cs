using AllaganItemSearch.Mediator;
using AllaganItemSearch.Windows;

using AllaganLib.Interface.FormFields;
using AllaganLib.Interface.Interfaces;

using DalaMock.Host.Mediator;

using Dalamud.Game.ClientState.Keys;
using Dalamud.Plugin.Services;

using ImGuiService = AllaganItemSearch.Services.ImGuiService;
using Loc = AllaganItemSearch.Localization.Localization;

namespace AllaganItemSearch.Settings;

public class MainWindowHotKeySetting : HotkeyFormField<Configuration>, ISetting, IRegularHotkey<Configuration>
{
    private readonly MediatorService mediatorService;

    public MainWindowHotKeySetting(ImGuiService imGuiService, MediatorService mediatorService, IKeyState keyState)
        : base(imGuiService, keyState)
    {
        this.mediatorService = mediatorService;
    }

    public override VirtualKey[] DefaultValue { get; set; } = [];

    public override string Key { get; set; } = "MainWindowHotKey";

    public override string Name { get; set; } = Loc.T("Hotkey - Main Window");

    public override string HelpText { get; set; } = Loc.T("The hotkey to open the main window");

    public override string Version { get; } = "1.0.0.5";

    public SettingType Type { get; set; } = SettingType.Features;

    public void OnTriggered()
    {
        this.mediatorService.Publish(new ToggleWindowMessage(typeof(MainWindow)));
    }
}
