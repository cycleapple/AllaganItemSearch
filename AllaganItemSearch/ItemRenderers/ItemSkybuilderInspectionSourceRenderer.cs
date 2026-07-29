using System;

using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.ItemSources;
using AllaganLib.GameSheets.Sheets;

using Dalamud.Bindings.ImGui;

namespace AllaganItemSearch.ItemRenderers;

public class ItemSkybuilderInspectionUseRenderer : ItemSkybuilderInspectionSourceRenderer
{
    public ItemSkybuilderInspectionUseRenderer(GatheringItemSheet gatheringItemSheet) : base(gatheringItemSheet)
    {
    }

    public override RendererType RendererType => RendererType.Use;
    public override string HelpText => "Is the item the approved form of a item that must be inspected at the firmament?";
    public override Action<ItemSource> DrawTooltip => source =>
    {
        var asSource = this.AsSource(source);
        ImGui.Text($"獎勵：{asSource.Item.NameString}");
        ImGui.Text($"需求數量：{asSource.InspectionData.AmountRequired}");
    };
}

public class ItemSkybuilderInspectionSourceRenderer : ItemInfoRenderer<ItemSkybuilderInspectionSource>
{
    private readonly GatheringItemSheet gatheringItemSheet;

    public ItemSkybuilderInspectionSourceRenderer(GatheringItemSheet gatheringItemSheet)
    {
        this.gatheringItemSheet = gatheringItemSheet;
    }
    public override RendererType RendererType => RendererType.Source;
    public override ItemInfoType Type => ItemInfoType.SkybuilderInspection;
    public override string SingularName => "Sky Builder Inspection";
    public override string HelpText => "Can the item be inspected at the firmament to convert it into the approved form?";
    public override bool ShouldGroup => true;

    public override Action<ItemSource> DrawTooltip => source =>
    {
        var asSource = this.AsSource(source);
        ImGui.Text($"需求物品：{asSource.CostItem?.NameString ?? "未知物品"}");
        ImGui.Text($"需求數量：{asSource.InspectionData.AmountRequired}");
    };

    public override Func<ItemSource, string> GetName => source =>
    {
        var asSource = this.AsSource(source);
        return asSource.Item.NameString;
    };
    public override Func<ItemSource, int> GetIcon => _ => Icons.SkybuildersScripIcon;
}
