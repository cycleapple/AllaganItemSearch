using System;

using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.ItemSources;
using AllaganLib.Shared.Extensions;

using Dalamud.Bindings.ImGui;

namespace AllaganItemSearch.ItemRenderers;

public class ItemStainUseRenderer : ItemInfoRenderer<ItemStainSource>
{
    public override RendererType RendererType => RendererType.Use;
    public override ItemInfoType Type => ItemInfoType.Stain;
    public override string SingularName => "Dye";
    public override string PluralName => "Dyeing";
    public override string HelpText => "Can the item be used to dye an item?";
    public override bool ShouldGroup => true;

    public override Action<ItemSource> DrawTooltip => source =>
    {
        var stainSource = this.AsSource(source);
        ImGui.Text("顏色：" + stainSource.Stain.Value.Name.ExtractText());
        var color = stainSource.Stain.Value.Color.ThreeChannelColorToVector4Color();
        if (ImGui.ColorButton("ColorPreview", color, ImGuiColorEditFlags.None, new (64,64)))
        {
        }
    };
    public override Func<ItemSource, string> GetName => source =>
    {
        var stainSource = this.AsSource(source);
        return stainSource.Stain.Value.Name.ExtractText();
    };
    public override Func<ItemSource, int> GetIcon => source =>
    {
        return Icons.DyeIcon;
    };
}
