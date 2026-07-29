using System;

using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.ItemSources;

using Dalamud.Bindings.ImGui;

namespace AllaganItemSearch.ItemRenderers;

public class ItemAquariumUseRenderer : ItemInfoRenderer<ItemAquariumSource>
{
    public override RendererType RendererType => RendererType.Use;
    public override ItemInfoType Type => ItemInfoType.Aquarium;
    public override string SingularName => "Aquarium";
    public override string PluralName => "Aquariums";
    public override string HelpText => "Can the item be placed in aquariums?";
    public override bool ShouldGroup => false;

    public override Action<ItemSource> DrawTooltip => source =>
    {
        var aquariumSource = this.AsSource(source);
        ImGui.Text("尺寸：" + aquariumSource.AquariumFish.Size);
        ImGui.Text("水質：" + aquariumSource.AquariumFish.Base.AquariumWater.Value.Name.ExtractText());
    };

    public override Func<ItemSource, string> GetName => source =>
    {
        var aquariumSource = this.AsSource(source);

        return "Aquarium: " + aquariumSource.AquariumFish.Base.AquariumWater.Value.Name.ExtractText() + " (" +
               aquariumSource.AquariumFish.Size + " )";
    };

    public override Func<ItemSource, int> GetIcon => _ => Icons.AquariumIcon;
}
