using System.Collections.Generic;

using AllaganLib.Interface.FormFields;
using AllaganLib.Interface.Services;
using Loc = AllaganItemSearch.Localization.Localization;

namespace AllaganItemSearch.Settings;

public enum FilterPanePosition
{
    Right,
    Left,
    Bottom,
    Top
}

public class FilterPanePositionSetting : ChoiceFormField<FilterPanePosition, Configuration>, ISetting
{
    public FilterPanePositionSetting(ImGuiService imGuiService) : base(imGuiService)
    {
    }

    public override FilterPanePosition DefaultValue { get; set; } = FilterPanePosition.Right;

    public override string Key { get; set; } = "FilterPanePosition";

    public override string Name { get; set; } = Loc.T("Filter Pane Position");

    public override string HelpText { get; set; } = Loc.T("What side should the filter pane be positioned?");

    public override bool Equal(FilterPanePosition item1, FilterPanePosition item2) => item1 == item2;

    public override string Version { get; } = "1.0.0.0";

    public override Dictionary<FilterPanePosition, string> Choices => new Dictionary<FilterPanePosition, string>
    {
        { FilterPanePosition.Left, Loc.T("Left") },
        { FilterPanePosition.Right, Loc.T("Right") },
        { FilterPanePosition.Bottom, Loc.T("Bottom") },
        { FilterPanePosition.Top, Loc.T("Top") },
    };

    public SettingType Type { get; set; } = SettingType.Layout;
}
