using System;

using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.ItemSources;

using Dalamud.Bindings.ImGui;

namespace AllaganItemSearch.ItemRenderers;

public class ItemGardeningCrossbreedSourceRenderer : ItemInfoRenderer<ItemGardeningCrossbreedSource>
{
    public override RendererType RendererType { get; } = RendererType.Source;

    public override ItemInfoType Type { get; } = ItemInfoType.GardeningCrossbreed;

    public override string SingularName { get; } = "Gardening Crossbreed";

    public override string HelpText { get; } = "Is this item created by crossbreeding 2 seeds?";

    public override bool ShouldGroup { get; } = true;

    public override Action<ItemSource> DrawTooltip => source =>
    {
        var asSource = this.AsSource(source);
        ImGui.Text($"結果：{asSource.SeedResult.NameString}");
        ImGui.Text($"{asSource.Seed1.NameString} + {asSource.Seed2.NameString}");
    };

    public override Func<ItemSource, string> GetName => source =>
    {
        var asSource = this.AsSource(source);
        return $"{asSource.SeedResult.NameString} - {asSource.Seed1.NameString} + {asSource.Seed2.NameString}";
    };

    public override Func<ItemSource, int> GetIcon => _ => Icons.SeedBagIcon;
}
