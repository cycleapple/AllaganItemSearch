using System;
using System.Collections.Generic;
using System.Linq;

using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.ItemSources;
using AllaganLib.GameSheets.Sheets;

using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

using Dalamud.Bindings.ImGui;

namespace AllaganItemSearch.ItemRenderers;

public class ItemCollectablesShopUseRenderer : ItemCollectablesShopSourceRenderer
{
    private readonly ItemSheet _itemSheet;
    public override string HelpText => "Can the item be spent at a collectables exchange shop?";
    public ItemCollectablesShopUseRenderer(MapSheet mapSheet, ItemSheet itemSheet)
        : base(mapSheet, itemSheet)
    {
        this._itemSheet = itemSheet;
    }

    public override Action<List<ItemSource>>? DrawTooltipGrouped => sources =>
    {
        var asSources = AsSource(sources);

        var maps = asSources.SelectMany(shopSource => shopSource.MapIds == null || shopSource.MapIds.Count == 0
            ? new List<string>()
            : shopSource.MapIds.Select(c => this.MapSheet.GetRow(c).FormattedName)).Distinct().ToList();

        ImGui.Text("可購買的物品：");

        using (ImRaii.PushIndent())
        {
            foreach (var asSource in asSources.DistinctBy(c => c.Item).Select(c => c.Item.NameString))
            {
                ImGui.TextUnformatted(asSource);
            }
        }
    };

    public override RendererType RendererType => RendererType.Use;
}

public class ItemCollectablesShopSourceRenderer : ItemInfoRenderer<ItemCollectablesShopSource>
{
    public MapSheet MapSheet { get; }
    private readonly ItemSheet _itemSheet;

    public ItemCollectablesShopSourceRenderer(MapSheet mapSheet, ItemSheet itemSheet)
    {
        this.MapSheet = mapSheet;
        this._itemSheet = itemSheet;
    }

    public override RendererType RendererType => RendererType.Source;
    public override ItemInfoType Type => ItemInfoType.CollectablesShop;
    public override string SingularName => "Collectables Exchange Shop";
    public override string PluralName => "Collectables Exchange Shops";
    public override string HelpText => "Can the item be purchased from a collectables exchange shop?";
    public override bool ShouldGroup => true;

    public override byte MaxColumns => 1;
    public override IReadOnlyList<ItemInfoRenderCategory> Categories => [ItemInfoRenderCategory.Shop];

    public override Action<ItemSource> DrawTooltip => source =>
    {
        var asSource = AsSource(source);
    };

    public override Func<ItemSource, string> GetName => source =>
    {
        var asSource = AsSource(source);
        if (asSource.MapIds == null || asSource.MapIds.Count == 0)
        {
            return asSource.CollectablesShop.Name;
        }

        var maps = asSource.MapIds.Distinct().Select(c => this.MapSheet.GetRow(c).FormattedName);
        return asSource.CollectablesShop.Name + "(" + maps + ")";
    };

    public override Func<ItemSource, int> GetIcon => _ => Icons.CollectableShopIcon;
}
