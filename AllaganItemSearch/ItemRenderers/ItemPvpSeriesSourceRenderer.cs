using System;

using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.ItemSources;
using AllaganLib.GameSheets.Sheets;

using Dalamud.Plugin;
using Dalamud.Plugin.Services;

using Dalamud.Bindings.ImGui;

namespace AllaganItemSearch.ItemRenderers;

public class ItemPvpSeriesSourceRenderer : ItemInfoRenderer<ItemPVPSeriesSource>
{
    public override RendererType RendererType { get; } = RendererType.Source;
    public override ItemInfoType Type { get; } = ItemInfoType.PVPSeries;
    public override string SingularName { get; } = "PVP Series";
    public override string HelpText { get; } = "Is this item rewarded from a PVP series?";
    public override bool ShouldGroup { get; } = true;

    public override Action<ItemSource> DrawTooltip => source =>
    {
        var asSource = this.AsSource(source);
        ImGui.Text("PvP 賽季 " + asSource.PvpSeries.Value.RowId + " 獎勵");
        ImGui.Text("於等級 " + asSource.Level + " 解鎖");
    };

    public override Func<ItemSource, string> GetName => source =>
    {
        var asSource = this.AsSource(source);
        return $"Series {asSource.PvpSeries.Value.RowId} Level {asSource.Level}";
    };

    public override Func<ItemSource, int> GetIcon => _ => Icons.PVPIcon;
}
