using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.ItemSources;

using Dalamud.Interface.Textures;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;

using Dalamud.Bindings.ImGui;

namespace AllaganItemSearch.ItemRenderers;

public class ItemCraftRequirementSourceRenderer : ItemInfoRenderer<ItemCraftRequirementSource>
{
    private readonly ITextureProvider textureProvider;

    public ItemCraftRequirementSourceRenderer(ITextureProvider textureProvider)
    {
        this.textureProvider = textureProvider;
    }
    public override RendererType RendererType => RendererType.Use;
    public override ItemInfoType Type => ItemInfoType.CraftRecipe;
    public override string SingularName => "Craft Ingredient";
    public override bool ShouldGroup => true;
    public override IReadOnlyList<ItemInfoRenderCategory> Categories => [ItemInfoRenderCategory.Crafting];
    public override string HelpText => "Can the item be used as a material in a craft recipe?";

    public override Func<List<ItemSource>, List<List<ItemSource>>>? CustomGroup => sources =>
    {
        return sources.GroupBy(c => this.AsSource(c).Recipe.Base.CraftType.RowId).Select(c => c.ToList()).ToList();
    };

    public override Action<ItemSource> DrawTooltip => source =>
    {
        var asSource = this.AsSource(source);
        ImGui.TextUnformatted("使用此材料的製作配方：");
        using (ImRaii.PushIndent())
        {
            ImGui.Image(this.textureProvider.GetFromGameIcon(new GameIconLookup(asSource.Item.Icon)).GetWrapOrEmpty().Handle, new Vector2(16,16));
            ImGui.SameLine();
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
