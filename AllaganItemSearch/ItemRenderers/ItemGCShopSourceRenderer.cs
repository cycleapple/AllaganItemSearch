using System;
using System.Collections.Generic;
using System.Linq;

using AllaganItemSearch.Services;

using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Extensions;
using AllaganLib.GameSheets.ItemSources;
using AllaganLib.GameSheets.Sheets;

using Dalamud.Interface.Utility.Raii;

using Dalamud.Bindings.ImGui;

using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace AllaganItemSearch.ItemRenderers;

public class ItemGcShopUseRenderer : ItemGcShopSourceRenderer
{
    private readonly ItemSheet itemSheet;

    public ItemGcShopUseRenderer(ImGuiService imGuiService, MapSheet mapSheet, ItemSheet itemSheet, ExcelSheet<GCRankGridaniaMaleText> rankSheet) : base(imGuiService, mapSheet, itemSheet, rankSheet)
    {
        this.itemSheet = itemSheet;
    }

    public override string HelpText => "Can the item be spent at a grand company shop?";

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

public class ItemGcShopSourceRenderer : ItemInfoRenderer<ItemGCShopSource>
{
    public MapSheet MapSheet { get; }
    private readonly ImGuiService imGuiService;
    private readonly ItemSheet itemSheet;
    private readonly ExcelSheet<GCRankGridaniaMaleText> rankSheet;

    public ItemGcShopSourceRenderer(ImGuiService imGuiService, MapSheet mapSheet, ItemSheet itemSheet, ExcelSheet<GCRankGridaniaMaleText> rankSheet)
    {
        this.MapSheet = mapSheet;
        this.imGuiService = imGuiService;
        this.itemSheet = itemSheet;
        this.rankSheet = rankSheet;
    }

    public override RendererType RendererType => RendererType.Source;
    public override ItemInfoType Type => ItemInfoType.GCShop;
    public override string SingularName => "Grand Company Shop";
    public override string PluralName => "Grand Company Shops";
    public override string HelpText => "Can the item be purchased at your grand company shop?";
    public override bool ShouldGroup => true;
    public override IReadOnlyList<ItemInfoRenderCategory> Categories => [ItemInfoRenderCategory.Shop];

    public override byte MaxColumns => 1;

    public override Action<ItemSource> DrawTooltip => source =>
    {
        var asSource = this.AsSource(source);
        var maps = asSource.MapIds?.Distinct().Select(c => this.MapSheet.GetRow(c).FormattedName).ToList() ?? new List<string>();

        ImGui.Text($"所需物品：{asSource.CostItem!.NameString} x {asSource.GCScripShopItem.Base.CostGCSeals}");
        if (asSource.GCScripShopItem.Base.RequiredGrandCompanyRank.IsValid)
        {
            var genericRank = this.rankSheet
                                  .GetRow(asSource.GCScripShopItem.Base.RequiredGrandCompanyRank.RowId).NameRank.ExtractText()
                                  .ToTitleCase();
            ImGui.Text($"軍階需求：{genericRank}");
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
            return asSource.GcShop.Name;
        }

        var maps = asSource.MapIds.Distinct().Select(c => this.MapSheet.GetRow(c).FormattedName);
        return asSource.GcShop.Name + "(" + maps + ")";
    };

    public override Func<ItemSource, int> GetIcon => source =>
    {
        var asSource = this.AsSource(source);
        var grandCompanyRowId = asSource.GCScripShopItem.Category.GrandCompany.RowId;
        switch (grandCompanyRowId)
        {

        }
        return Icons.GrandCompany3;
    };
}
