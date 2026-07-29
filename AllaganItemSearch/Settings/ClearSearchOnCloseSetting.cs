using AllaganItemSearch.Services;

using AllaganLib.Interface.FormFields;
using Loc = AllaganItemSearch.Localization.Localization;

namespace AllaganItemSearch.Settings;

public class ClearSearchOnCloseSetting(ImGuiService imGuiService) : BooleanFormField<Configuration>(imGuiService), ISetting
{
    public override bool DefaultValue { get; set; } = true;

    public override string Key { get; set; } = "ClearSearchOnClose";

    public override string Name { get; set; } = Loc.T("Clear Search on Close?");

    public override string HelpText { get; set; } = Loc.T("Should the search be cleared when the main window is closed?");

    public override string Version { get; } = "1.0.0.5";

    public SettingType Type { get; set; } = SettingType.General;
}
