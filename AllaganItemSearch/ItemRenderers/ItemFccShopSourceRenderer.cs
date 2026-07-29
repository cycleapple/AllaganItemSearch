using System;
using System.Collections.Generic;
using System.Linq;

using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.ItemSources;
using AllaganLib.GameSheets.Sheets;

using Dalamud.Interface.Utility.Raii;

using Dalamud.Bindings.ImGui;

namespace AllaganItemSearch.ItemRenderers;

public class ItemFccShopUseRenderer : ItemFccShopSourceRenderer
{
    private readonly ItemSheet _itemSheet;
    public override string HelpText => "Can the item be spent at a free company shop?";
    public ItemFccShopUseRenderer(MapSheet mapSheet) : base(mapSheet)
    {
    }

    public override Action<List<ItemSource>>? DrawTooltipGrouped => sources =>
    {
        var asSources = this.AsSource(sources);

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

        if (maps.Count != 0)
        {
        ImGui.Text("地圖：");
            using (ImRaii.PushIndent())
            {
                foreach (var map in maps)
                {
                    ImGui.Text(map);
                }
            }
        }
    };

    public override RendererType RendererType => RendererType.Use;
}

public class ItemFccShopSourceRenderer : ItemInfoRenderer<ItemFccShopSource>
{
    public MapSheet MapSheet { get; }

    public ItemFccShopSourceRenderer(MapSheet mapSheet)
    {
        this.MapSheet = mapSheet;
    }

    public override RendererType RendererType => RendererType.Source;
    public override ItemInfoType Type => ItemInfoType.FCShop;
    public override string SingularName => "Free Company Shop";
    public override string PluralName => "Free Company Shops";
    public override string HelpText => "Can the item be purchased from a free company shop?";
    public override bool ShouldGroup => true;

    public override byte MaxColumns => 1;
    public override IReadOnlyList<ItemInfoRenderCategory> Categories => [ItemInfoRenderCategory.Shop];

    public override Action<List<ItemSource>>? DrawTooltipGrouped => sources =>
    {
        var asSources = this.AsSource(sources);
        var maps = asSources.SelectMany(shopSource => shopSource.MapIds == null || shopSource.MapIds.Count == 0
            ? new List<string>()
            : shopSource.MapIds.Select(c => this.MapSheet.GetRow(c).FormattedName)).Distinct().ToList();

        using (ImRaii.PushIndent())
        {
        ImGui.Text($"所需物品：公會點數 x {asSources.First().FccShopListing.Cost.Count}");
        }

        if (maps.Count != 0)
        {
        ImGui.Text("地圖：");
            using (ImRaii.PushIndent())
            {
                foreach (var map in maps)
                {
                    ImGui.Text(map);
                }
            }
        }
    };

    public override Action<ItemSource> DrawTooltip => source =>
    {
        var asSource = this.AsSource(source);
        var maps = asSource.MapIds?.Distinct().Select(c => this.MapSheet.GetRow(c).FormattedName).ToList() ?? new List<string>();

        using (ImRaii.PushIndent())
        {
        ImGui.Text($"所需物品：公會點數 x {asSource.FccShopListing.Cost.Count}");
        }

        if (maps.Count != 0)
        {
        ImGui.Text("地圖：");
            using (ImRaii.PushIndent())
            {
                foreach (var map in maps)
                {
                    ImGui.Text(map);
                }
            }
        }
    };

    public override Func<ItemSource, string> GetName => source =>
    {
        var asSource = this.AsSource(source);
        if (asSource.MapIds == null || asSource.MapIds.Count == 0)
        {
            return asSource.FccShop.Name;
        }

        var maps = asSource.MapIds.Distinct().Select(c => this.MapSheet.GetRow(c).FormattedName);
        return asSource.FccShop.Name + "(" + maps + ")";
    };

    public override Func<ItemSource, int> GetIcon => _ => Icons.FreeCompanyCreditIcon;
}
