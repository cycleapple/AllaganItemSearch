using System.Collections.Generic;

using static AllaganItemSearch.Settings.SettingType;
using Loc = AllaganItemSearch.Localization.Localization;

namespace AllaganItemSearch.Settings;

public class SettingTypeConfiguration
{
    private List<SettingType>? settingTypes;

    public static string GetFormattedName(SettingType settingType)
    {
        return settingType switch
        {
            General => Loc.T("General"),
            SettingType.Features => Loc.T("Features"),
            Mqtt => "MQTT",
            _ => Loc.T(settingType.ToString()),
        };
    }

    public List<SettingType> GetCategoryOrder()
    {
        return this.settingTypes ??= [General, SettingType.Features, Mqtt];
    }
}
