using System;
using System.Collections.Generic;

using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.ItemSources;

using Dalamud.Bindings.ImGui;

namespace AllaganItemSearch.ItemRenderers;

public class ItemFurnitureSourceRenderer : ItemInfoRenderer<ItemFurnitureSource>
{
    public override RendererType RendererType => RendererType.Use;
    public override ItemInfoType Type => ItemInfoType.FurnitureItem;
    public override string SingularName => "Interior Furniture";
    public override string HelpText => "Can the item be placed inside houses?";
    public override bool ShouldGroup => true;
    public override IReadOnlyList<ItemInfoRenderCategory>? Categories => [ItemInfoRenderCategory.House];

    public override Action<ItemSource> DrawTooltip => source =>
    {
        var asSource = this.AsSource(source);
        ImGui.Text($"分類：{asSource.FurnitureCatalogItem.Value.Category.Value.Category.ExtractText()}");
        ImGui.Text($"新增版本：{asSource.FurnitureCatalogItem.Value.Patch}");
    };

    public override Func<ItemSource, string> GetName => source =>
    {
        return source.Item.NameString;
    };
    public override Func<ItemSource, int> GetIcon => source => Icons.TableIcon;
}
