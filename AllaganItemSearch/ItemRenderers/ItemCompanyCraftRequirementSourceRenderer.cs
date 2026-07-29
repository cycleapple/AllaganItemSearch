using System;
using System.Collections.Generic;
using System.Linq;

using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.ItemSources;

using Dalamud.Interface.Utility.Raii;

using Dalamud.Bindings.ImGui;

namespace AllaganItemSearch.ItemRenderers;

public class ItemCompanyCraftRequirementSourceRenderer : ItemInfoRenderer<ItemCompanyCraftRequirementSource>
{
    public override RendererType RendererType => RendererType.Use;
    public override ItemInfoType Type => ItemInfoType.FreeCompanyCraftRecipe;
    public override string SingularName => "Company Craft Ingredient";
    public override bool ShouldGroup => true;
    public override string HelpText => "Is the item a material in a company craft recipe?";
    public override IReadOnlyList<ItemInfoRenderCategory> Categories => [ItemInfoRenderCategory.Crafting];

    public override Action<ItemSource> DrawTooltip => source =>
    {
        var asSource = this.AsSource(source);
        ImGui.TextUnformatted("使用此材料的製作配方：");
        using (ImRaii.PushIndent())
        {
            ImGui.TextUnformatted(this.GetName(source));
        }
    };

    public override Action<List<ItemSource>>? DrawTooltipGrouped => source =>
    {
        var asSource = this.AsSource(source);
        asSource = asSource.DistinctBy(c => c.Item.RowId).ToList();
        ImGui.TextUnformatted("使用此材料的製作配方：");
        using (ImRaii.PushIndent())
        {
            foreach (var row in asSource)
            {
                ImGui.TextUnformatted(this.GetName(row));
            }
        }
    };
    
    public override Func<ItemSource, string> GetName => source =>
    {
        var asSource = this.AsSource(source);
        return asSource.Item.NameString + " (" + (asSource.CompanyCraftSequence.Base.CompanyCraftType.ValueNullable?.Name.ExtractText() ?? "Unknown") + ")";
    };

    public override Func<ItemSource, int> GetIcon => _ => Icons.CraftIcon;
}
