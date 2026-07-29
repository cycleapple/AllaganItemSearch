using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using AllaganItemSearch.Services;

using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.ItemSources;
using AllaganLib.GameSheets.Sheets;

using Dalamud.Interface.Textures;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;

using Dalamud.Bindings.ImGui;

namespace AllaganItemSearch.ItemRenderers;

public class ItemSpecialShopUseRenderer : ItemSpecialShopSourceRenderer
{
    public ItemSpecialShopUseRenderer(ITextureProvider textureProvider, ImGuiService imGuiService, MapSheet mapSheet)
        : base(textureProvider, imGuiService, mapSheet)
    {
    }

    public override RendererType RendererType => RendererType.Use;

    public override string HelpText => "Can the item be spent at a special currency shop?";

    public override Func<ItemSource, int> GetIcon => source =>
    {
        var asSource = this.AsSource(source);
        return asSource.Item.Icon;
    };
}

public class ItemSpecialShopSourceRenderer : ItemInfoRenderer<ItemSpecialShopSource>
{
    private readonly ITextureProvider textureProvider;
    private readonly ImGuiService imGuiService;
    private readonly MapSheet mapSheet;

    public ItemSpecialShopSourceRenderer(ITextureProvider textureProvider, ImGuiService imGuiService, MapSheet mapSheet)
    {
        this.textureProvider = textureProvider;
        this.imGuiService = imGuiService;
        this.mapSheet = mapSheet;
    }

    public override RendererType RendererType => RendererType.Source;
    public override ItemInfoType Type => ItemInfoType.SpecialShop;
    public override string SingularName => "Special Shop";
    public override string PluralName => "Special Shops";
    public override string HelpText => "Can the item be purchased from a special currency shop?";
    public override bool ShouldGroup => true;

    public override byte MaxColumns => 1;
    public override IReadOnlyList<ItemInfoRenderCategory> Categories => [ItemInfoRenderCategory.Shop];

    public override Func<List<ItemSource>, List<List<ItemSource>>>? CustomGroup => sources =>
    {
        return sources.GroupBy(c => (c.CostItems.Count, c.CostItems.FirstOrDefault()?.ItemId ?? null)).Select(c => c.ToList()).ToList();
    };

    public override Action<ItemSource> DrawTooltip => source =>
    {
        var asSource = this.AsSource(source);
        var maps = asSource.MapIds == null || asSource.MapIds.Count == 0
            ? null
            : asSource.MapIds.Select(c => this.mapSheet.GetRow(c).FormattedName);

        ImGui.Text($"商店：{asSource.Shop.Name}");
        ImGui.Text("獎勵：");
        using (ImRaii.PushIndent())
        {
            foreach (var reward in asSource.ShopListing.Rewards)
            {
                ImGui.Image(this.textureProvider.GetFromGameIcon(new GameIconLookup(reward.Item.Icon)).GetWrapOrEmpty().Handle, new Vector2(18, 18) * ImGui.GetIO().FontGlobalScale);
                ImGui.SameLine();
                var itemName = reward.Item.NameString;
                var count = reward.Count;
                var costString = $"{itemName} x {count}";
                ImGui.Text(costString);
                if (reward.IsHq == true)
                {
                    ImGui.Image(
                        this.imGuiService.LoadImage("hq").GetWrapOrEmpty().Handle,
                        new Vector2(18, 18) * ImGui.GetIO().FontGlobalScale);
                }
            }
        }
        ImGui.Text("所需物品：");
        using (ImRaii.PushIndent())
        {
            foreach (var cost in asSource.ShopListing.Costs)
            {
                ImGui.Image(this.textureProvider.GetFromGameIcon(new GameIconLookup(cost.Item.Icon)).GetWrapOrEmpty().Handle, new Vector2(18, 18) * ImGui.GetIO().FontGlobalScale);
                ImGui.SameLine();
                var itemName = cost.Item.NameString;
                var count = cost.Count;
                var costString = $"{itemName} x {count}";
                ImGui.Text(costString);
                if (cost.IsHq == true)
                {
                    ImGui.Image(
                        this.imGuiService.LoadImage("hq").GetWrapOrEmpty().Handle,
                        new Vector2(18, 18) * ImGui.GetIO().FontGlobalScale);
                }
            }
        }

        if (maps != null)
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
        var costs = String.Join(", ",
            asSource.ShopListing.Costs.Select(c => c.Item.NameString + " (" + c.Count + ")"));
        if (asSource.ShopListing.Rewards.Count() > 1)
        {
            var rewards = String.Join(", ",
                asSource.ShopListing.Rewards.Select(c => c.Item.NameString + " (" + c.Count + ")"));
            return $"Costs {costs} - Rewards {rewards}";
        }

        return $"Costs {costs}";
    };

    public override Func<ItemSource, int> GetIcon => source =>
    {
        var asSource = this.AsSource(source);
        return asSource.CostItems.FirstOrDefault()?.ItemRow.Icon ?? asSource.Item.Icon;
    };
}
