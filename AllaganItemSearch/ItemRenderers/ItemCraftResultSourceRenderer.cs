using System;
using System.Collections.Generic;
using System.Linq;

using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.ItemSources;
using AllaganLib.GameSheets.Sheets;

using Dalamud.Interface.Utility.Raii;

using Dalamud.Bindings.ImGui;

namespace AllaganItemSearch.ItemRenderers;

public class ItemCraftResultSourceRenderer : ItemInfoRenderer<ItemCraftResultSource>
{
    private readonly ItemSheet itemSheet;

    public ItemCraftResultSourceRenderer(ItemSheet itemSheet)
    {
        this.itemSheet = itemSheet;
    }

    public override RendererType RendererType => RendererType.Source;
    public override ItemInfoType Type => ItemInfoType.CraftRecipe;
    public override string SingularName => "Craft Recipe";
    public override bool ShouldGroup => true;
    public override IReadOnlyList<ItemInfoRenderCategory> Categories => [ItemInfoRenderCategory.Crafting];
    public override string HelpText => "Can the item be crafted via a craft recipe?";

    public override Func<List<ItemSource>, List<List<ItemSource>>>? CustomGroup => sources =>
    {
        return sources.GroupBy(c => this.AsSource(c).Recipe.Base.CraftType.RowId).Select(c => c.ToList()).ToList();
    };

    public override Action<ItemSource> DrawTooltip => source =>
    {
        var asSource = this.AsSource(source);
        ImGui.Text($"製作類型：{asSource.Recipe.Base.CraftType.Value.Name}");
        ImGui.Text($"產量：{asSource.Recipe.Base.AmountResult}");
        ImGui.Text($"難度：{asSource.Recipe.Base.DifficultyFactor}");
        ImGui.Text($"作業精度需求：{asSource.Recipe.Base.RequiredCraftsmanship}");

        ImGui.Text("材料：");
        using (ImRaii.PushIndent())
        {
            foreach (var ingredient in asSource.Recipe.IngredientCounts)
            {
                var item = this.itemSheet.GetRow(ingredient.Key);
                ImGui.Text($"{item.NameString} x {ingredient.Value}");
            }
        }
    };
    public override Func<ItemSource, string> GetName => source =>
    {
        var asSource = this.AsSource(source);
        return asSource.Item.NameString + " (" + (asSource.Recipe.CraftType?.FormattedName ?? "Unknown") + ")";
    };

    public override Func<ItemSource, int> GetIcon => source =>
    {
        var asSource = this.AsSource(source);
        if (asSource.Recipe.CraftType != null)
        {
            return asSource.Recipe.CraftType.Icon;
        }

        return Icons.CraftIcon;
    };
}
