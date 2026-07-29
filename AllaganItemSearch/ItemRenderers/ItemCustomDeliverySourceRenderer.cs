using System;

using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.ItemSources;

using Dalamud.Bindings.ImGui;

namespace AllaganItemSearch.ItemRenderers;

public class ItemCustomDeliverySourceRenderer : ItemInfoRenderer<ItemCustomDeliverySource>
{
    public override RendererType RendererType => RendererType.Use;
    public override ItemInfoType Type => ItemInfoType.CustomDelivery;
    public override string SingularName => "Custom Delivery";
    public override string PluralName => "Custom Deliveries";
    public override string HelpText => "Can the item be delivered in a custom delivery quest?";
    public override bool ShouldGroup => false;

    public override Action<ItemSource> DrawTooltip => source =>
    {
        var asSource = this.AsSource(source);
        var eNpcResident = asSource.SupplyRow.Npc?.Base.Npc.Value;
        if (eNpcResident != null)
        {
            var collectabilityLow = asSource.SupplyRow.Base.CollectabilityLow;
            var collectabilityMid = asSource.SupplyRow.Base.CollectabilityMid;
            var collectabilityHigh = asSource.SupplyRow.Base.CollectabilityHigh;
        ImGui.Text("NPC：" + eNpcResident.Value.Singular.ExtractText());
        ImGui.Text("收藏價值（低）： " + collectabilityLow);
        ImGui.Text("收藏價值（中）： " + collectabilityMid);
        ImGui.Text("收藏價值（高）： " + collectabilityHigh);
        }
        else
        {
        ImGui.Text("未知 NPC");
        }
    };

    public override Func<ItemSource, string> GetName => source =>
    {
        var asSource = this.AsSource(source);
        var eNpcResident = asSource.SupplyRow.Npc?.Base.Npc.Value;
        if (eNpcResident != null)
        {
            return eNpcResident.Value.Singular.ExtractText();
        }

        return "";
    };

    public override Func<ItemSource, int> GetIcon => _ => Icons.CustomDeliveryIcon;
}
