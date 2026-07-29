using System;

using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.ItemSources;

using Dalamud.Interface.Utility.Raii;

using Dalamud.Bindings.ImGui;

namespace AllaganItemSearch.ItemRenderers;

public class ItemGlamourReadySetSourceRenderer : ItemInfoRenderer<ItemGlamourReadySetSource>
{
    public override RendererType RendererType => RendererType.Use;
    public override ItemInfoType Type => ItemInfoType.GlamourReadySet;
    public override string SingularName => "Glamour Ready Set";
    public override string HelpText => "Is the item the combined item of a glamour ready item set?";

    public override bool ShouldGroup => true;

    public override Action<ItemSource> DrawTooltip => source =>
    {
        var asSource = this.AsSource(source);
        if (asSource.SetItems.Count > 1)
        {
        ImGui.Text("套裝物品：");
            using (ImRaii.PushIndent())
            {
                foreach (var item in asSource.SetItems)
                {
                    ImGui.Text(item.NameString);
                }
            }
        }
    };

    public override Func<ItemSource, string> GetName => source => "";
    public override Func<ItemSource, int> GetIcon => _ => Icons.MannequinIcon;
}
