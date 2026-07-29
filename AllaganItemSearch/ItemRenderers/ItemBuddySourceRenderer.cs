using System;
using System.Collections.Generic;

using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.ItemSources;

using Dalamud.Bindings.ImGui;

namespace AllaganItemSearch.ItemRenderers;

public class ItemBuddySourceRenderer : ItemInfoRenderer<ItemBuddySource>
{
    public override RendererType RendererType => RendererType.Use;
    public override ItemInfoType Type => ItemInfoType.BuddyItem;
    public override string SingularName => "Used on Chocobo Companion";
    public override bool ShouldGroup => false;
    public override string HelpText => "Can the item be used on your chocobo companion?";

    public override Action<ItemSource> DrawTooltip => source =>
    {
        var asSource = this.AsSource(source);
        var usedField = asSource.BuddyItem.Value.UseField;
        var usedTraining = asSource.BuddyItem.Value.UseTraining;
        var usedDyeing = asSource.BuddyItem.Value.Unknown0;

        if (usedField)
        {
            ImGui.Text("戰鬥：提高搭檔陸行鳥取得的經驗值。");
        }

        if (usedTraining)
        {
            ImGui.Text("陸行鳥房：用於訓練寄養中的搭檔陸行鳥。");
        }

        if (usedDyeing)
        {
            ImGui.Text("染色：用於改變陸行鳥羽毛顏色。");
        }
    };
    public override Func<ItemSource, string> GetName => source =>
    {
        var asSource = this.AsSource(source);
        var usedField = asSource.BuddyItem.Value.UseField;
        var usedTraining = asSource.BuddyItem.Value.UseTraining;
        var usedDyeing = asSource.BuddyItem.Value.Unknown0;
        var name = new List<string>();


        if (usedField)
        {
            name.Add("battle");
        }

        if (usedTraining)
        {
            name.Add("training");
        }

        if (usedDyeing)
        {
            name.Add("dyeing");
        }

        return "chocobo " + string.Join(", ", name);
    };

    public override Func<ItemSource, int> GetIcon => _ => Icons.ChocoboIcon;
}
