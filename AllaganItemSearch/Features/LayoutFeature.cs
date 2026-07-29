using System.Collections.Generic;

using AllaganItemSearch.Settings;

using AllaganLib.Interface.FormFields;
using AllaganLib.Interface.Wizard;
using Loc = AllaganItemSearch.Localization.Localization;

namespace AllaganItemSearch.Features;

public class LayoutFeature(IEnumerable<IFormField<Configuration>> settings) : Feature<Configuration>(
    [
        typeof(FilterPanePositionSetting),
    ],
    settings)
{
    public override string Name { get; } = Loc.T("Layout");

    public override string Description { get; } = Loc.T("Customize the plugins layout.");
}
