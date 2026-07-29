using System;
using System.Collections.Generic;
using System.Linq;

using AllaganItemSearch.Services;

using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.ItemSources;
using AllaganLib.GameSheets.Sheets;

using Dalamud.Interface.Utility.Raii;

using Dalamud.Bindings.ImGui;

namespace AllaganItemSearch.ItemRenderers;

public class ItemCalamitySalvagerShopUseRenderer : ItemCalamitySalvagerShopSourceRenderer
{
    private readonly ItemSheet itemSheet;

    public override string HelpText => "Can the item be spent at the calamity salvager?";

    public ItemCalamitySalvagerShopUseRenderer(ImGuiService imGuiService, MapSheet mapSheet, ItemSheet itemSheet) : base(imGuiService, mapSheet, itemSheet)
    {
        this.itemSheet = itemSheet;
    }

    public override RendererType RendererType => RendererType.Use;
}

public class ItemCalamitySalvagerShopSourceRenderer : ItemInfoRenderer<ItemCalamitySalvagerShopSource>
{
    private readonly ImGuiService imGuiService;
    private readonly MapSheet mapSheet;
    private readonly ItemSheet itemSheet;

    public ItemCalamitySalvagerShopSourceRenderer(ImGuiService imGuiService, MapSheet mapSheet, ItemSheet itemSheet)
    {
        this.imGuiService = imGuiService;
        this.mapSheet = mapSheet;
        this.itemSheet = itemSheet;
    }

    public override IReadOnlyList<ItemInfoRenderCategory> Categories => [ItemInfoRenderCategory.Shop];
    public override RendererType RendererType => RendererType.Source;
    public override ItemInfoType Type => ItemInfoType.CalamitySalvagerShop;
    public override string SingularName => "Calamity Salvager";
    public override string PluralName => "Calamity Salvagers";
    public override bool ShouldGroup => true;
    public override string HelpText => "Can the item be purchased from the Calamity Salvager?";

    public override byte MaxColumns => 1;

    public override Action<List<ItemSource>>? DrawTooltipGrouped => sources =>
    {
        var asSources = this.AsSource(sources);
        var firstItem = asSources.First();
        var allCalamitySalvagerShops = asSources.Cast<ItemCalamitySalvagerShopSource>().ToList();
        var maps = allCalamitySalvagerShops.SelectMany(shopSource => shopSource.MapIds == null || shopSource.MapIds.Count == 0
            ? new List<string>()
            : shopSource.MapIds.Select(c => this.mapSheet.GetRow(c).FormattedName)).ToList();

        ImGui.Text("所需物品：");

        using (ImRaii.PushIndent())
        {
            var itemName = firstItem.CostItem!.NameString;
            var count = firstItem.Cost;
            var costString = $"{itemName} x {count}";
            ImGui.Text(costString);

            if (firstItem.GilShopItem.Base.AchievementRequired.RowId != 0)
            {
                ImGui.Text(
                    $"Achievement Required: {firstItem.GilShopItem.Base.AchievementRequired.Value.Name.ExtractText()}");
            }

            foreach (var quest in firstItem.GilShopItem.Base.QuestRequired)
            {
                if (quest.RowId != 0)
                {
                    ImGui.Text(
                        $"Quest Required: {quest.Value.Name.ExtractText()}");
                }
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

    public override Action<ItemSource> DrawTooltip => source =>
    {
        var asSource = this.AsSource(source);
        var maps = source.MapIds?.Select(c => this.mapSheet.GetRow(c).FormattedName).ToList() ?? new List<string>();

        ImGui.Text("所需物品：");

        using (ImRaii.PushIndent())
        {
            var itemName = asSource.CostItem!.NameString;
            var count = asSource.Cost;
            var costString = $"{itemName} x {count}";
            ImGui.Text(costString);

            if (asSource.GilShopItem.Base.AchievementRequired.RowId != 0)
            {
                ImGui.Text(
                    $"Achievement Required: {asSource.GilShopItem.Base.AchievementRequired.Value.Name.ExtractText()}");
            }

            foreach (var quest in asSource.GilShopItem.Base.QuestRequired)
            {
                if (quest.RowId != 0)
                {
                    ImGui.Text(
                        $"Quest Required: {quest.Value.Name.ExtractText()}");
                }
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

    public override Func<ItemSource, string> GetName => source =>
    {
        var asSource = this.AsSource(source);
        if (asSource.MapIds == null || asSource.MapIds.Count == 0)
        {
            return asSource.GilShop.Name;
        }

        var maps = asSource.MapIds.Select(c => this.mapSheet.GetRow(c).FormattedName);
        return asSource.GilShop.Name + "(" + maps + ")";
    };

    public override Func<ItemSource, int> GetIcon => source => Icons.CalamitySalvagerBag;
}
