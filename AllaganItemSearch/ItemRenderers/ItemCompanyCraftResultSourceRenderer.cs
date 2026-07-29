using System;
using System.Collections.Generic;

using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.ItemSources;
using AllaganLib.GameSheets.Sheets;

using Dalamud.Interface.Utility.Raii;

using Dalamud.Bindings.ImGui;

namespace AllaganItemSearch.ItemRenderers;

public class ItemCompanyCraftResultSourceRenderer : ItemInfoRenderer<ItemCompanyCraftResultSource>
{
    private readonly ItemSheet itemSheet;

    public ItemCompanyCraftResultSourceRenderer(ItemSheet itemSheet)
    {
        this.itemSheet = itemSheet;
    }

    public override RendererType RendererType => RendererType.Source;
    public override ItemInfoType Type => ItemInfoType.FreeCompanyCraftRecipe;
    public override string SingularName => "Company Craft";
    public override bool ShouldGroup => true;
    public override IReadOnlyList<ItemInfoRenderCategory> Categories => [ItemInfoRenderCategory.Crafting];
    public override string HelpText => "Is the item crafted at the company workshop as a company craft recipe?";
    public override Action<ItemSource> DrawTooltip => source =>
    {
        var asSource = this.AsSource(source);
        ImGui.Text($"製作類型：{asSource.CompanyCraftSequence.Base.CompanyCraftType.Value.Name}");
        ImGui.Text($"部件數：{asSource.CompanyCraftSequence.CompanyCraftParts.Length}");
        ImGui.Text("材料：");
        using (ImRaii.PushIndent())
        {
            foreach (var ingredient in asSource.CompanyCraftSequence.MaterialsRequired(null))
            {
                var item = this.itemSheet.GetRow(ingredient.ItemId);
                ImGui.Text($"{item.NameString} x {ingredient.Quantity}");
            }
        }
    };

    public override Func<ItemSource, string> GetName => source =>
    {
        var asSource = this.AsSource(source);
        return asSource.Item.Base.Name.ExtractText() + "(" +
               (asSource.CompanyCraftSequence.Base.CompanyCraftType.ValueNullable?.Name.ExtractText() ?? "Unknown") +
               ")";
    };

    public override Func<ItemSource, int> GetIcon => _ => Icons.CraftIcon;
}
