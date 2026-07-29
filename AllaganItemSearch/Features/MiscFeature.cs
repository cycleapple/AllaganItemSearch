using System.Collections.Generic;

using AllaganItemSearch.Settings;

using AllaganLib.Interface.FormFields;
using AllaganLib.Interface.Wizard;
using Loc = AllaganItemSearch.Localization.Localization;

namespace AllaganItemSearch.Features;

public class MiscFeature(IEnumerable<IFormField<Configuration>> settings) : Feature<Configuration>(
    [
        typeof(AddTitleMenuButtonSetting),
    ],
    settings)
{
    public override string Name { get; } = Loc.T("Misc Features");

    public override string Description { get; } = Loc.T("Miscellaneous features that improve your overall experience.");
}
