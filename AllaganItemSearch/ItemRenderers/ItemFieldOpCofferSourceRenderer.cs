using System;
using System.Collections.Generic;

using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.ItemSources;
using AllaganLib.GameSheets.Sheets;

using Dalamud.Plugin;
using Dalamud.Plugin.Services;

using Dalamud.Bindings.ImGui;

namespace AllaganItemSearch.ItemRenderers;

public abstract class ItemFieldOpCofferSourceRenderer<T> : ItemInfoRenderer<T> where T : ItemFieldOpCofferSource
{
    private readonly ItemInfoType _itemInfoType;

    public ItemFieldOpCofferSourceRenderer(ItemSheet itemSheet, MapSheet mapSheet, ITextureProvider textureProvider, IDalamudPluginInterface pluginInterface, ItemInfoType itemInfoType)
    {
        this._itemInfoType = itemInfoType;
    }

    public override RendererType RendererType => RendererType.Source;
    public override ItemInfoType Type => this._itemInfoType;
    public override bool ShouldGroup => true;
    public override Action<ItemSource> DrawTooltip => source =>
    {
        var asSource = this.AsSource(source);
        ImGui.Text("掉落自" + asSource.CofferType + "寶箱");
        if (asSource.Min != null && asSource.Max != null)
        {
            ImGui.SameLine();
            if (asSource.Min == asSource.Max)
            {
            ImGui.Text("（掉落 1 個）");
            }
            else
            {
            ImGui.Text("（掉落 " + asSource.Min.Value + "～" + asSource.Max.Value + " 個）");
            }
        }

        if (asSource.Probability != null)
        {
            ImGui.SameLine();
            ImGui.TextUnformatted($"{asSource.Probability.Value}%");
        }
    };

    public override Func<ItemSource, int> GetIcon => _ => Icons.GoldChest2;

    public override Func<ItemSource, string> GetName => _ => "";
}

public class ItemPagosTreasureSourceRenderer : ItemFieldOpCofferSourceRenderer<ItemPagosTreasureCofferSource>
{
    public override IReadOnlyList<ItemInfoRenderCategory>? Categories { get; } =
        [ItemInfoRenderCategory.FieldOperation, ItemInfoRenderCategory.Pagos];

    public ItemPagosTreasureSourceRenderer(ItemSheet itemSheet, MapSheet mapSheet, ITextureProvider textureProvider, IDalamudPluginInterface pluginInterface) : base(itemSheet, mapSheet, textureProvider, pluginInterface, ItemInfoType.PagosTreasure)
    {
    }

    public override string SingularName => "Eureka Pagos (Treasure Coffer)";
    public override string HelpText => "Does this item drop from a pagos treasure coffer?";
}

public class ItemPyrosTreasureSourceRenderer : ItemFieldOpCofferSourceRenderer<ItemPyrosTreasureCofferSource>
{
    public override IReadOnlyList<ItemInfoRenderCategory>? Categories { get; } =
        [ItemInfoRenderCategory.FieldOperation, ItemInfoRenderCategory.Pyros];

    public ItemPyrosTreasureSourceRenderer(ItemSheet itemSheet, MapSheet mapSheet, ITextureProvider textureProvider, IDalamudPluginInterface pluginInterface) : base(itemSheet, mapSheet, textureProvider, pluginInterface, ItemInfoType.PyrosTreasure)
    {
    }

    public override string SingularName => "Eureka Pyros (Treasure Coffer)";
    public override string HelpText => "Does this item drop from a pyros treasure coffer?";
}

public class ItemHydatosTreasureSourceRenderer : ItemFieldOpCofferSourceRenderer<ItemHydatosTreasureCofferSource>
{
    public override IReadOnlyList<ItemInfoRenderCategory>? Categories { get; } =
        [ItemInfoRenderCategory.FieldOperation, ItemInfoRenderCategory.Hydatos];

    public ItemHydatosTreasureSourceRenderer(ItemSheet itemSheet, MapSheet mapSheet, ITextureProvider textureProvider, IDalamudPluginInterface pluginInterface) : base(itemSheet, mapSheet, textureProvider, pluginInterface, ItemInfoType.HydatosTreasure)
    {
    }

    public override string SingularName => "Eureka Hydatos (Treasure Coffer)";
    public override string HelpText => "Does this item drop from a hydatos treasure coffer?";
}

public class ItemOccultTreasureSourceRenderer : ItemFieldOpCofferSourceRenderer<ItemOccultTreasureCofferSource>
{
    public override IReadOnlyList<ItemInfoRenderCategory>? Categories { get; } =
        [ItemInfoRenderCategory.FieldOperation, ItemInfoRenderCategory.OccultCrescent];

    public ItemOccultTreasureSourceRenderer(ItemSheet itemSheet, MapSheet mapSheet, ITextureProvider textureProvider, IDalamudPluginInterface pluginInterface) : base(itemSheet, mapSheet, textureProvider, pluginInterface, ItemInfoType.OccultTreasure)
    {
    }

    public override string SingularName => "Occult Crescent (Treasure Coffer)";
    public override string HelpText => "Does this item drop from a occult crescent treasure coffer?";
}

public class ItemOccultPotSourceRenderer : ItemFieldOpCofferSourceRenderer<ItemOccultPotSource>
{
    public override IReadOnlyList<ItemInfoRenderCategory>? Categories { get; } =
        [ItemInfoRenderCategory.FieldOperation, ItemInfoRenderCategory.OccultCrescent];

    public ItemOccultPotSourceRenderer(ItemSheet itemSheet, MapSheet mapSheet, ITextureProvider textureProvider, IDalamudPluginInterface pluginInterface) : base(itemSheet, mapSheet, textureProvider, pluginInterface, ItemInfoType.OccultPot)
    {
    }

    public override string SingularName => "Occult Crescent (Pot)";
    public override string HelpText => "Does this item drop from a occult crescent pot?";
}

public class ItemOccultGoldenCofferSourceRenderer : ItemFieldOpCofferSourceRenderer<ItemOccultGoldenCofferSource>
{
    public override IReadOnlyList<ItemInfoRenderCategory>? Categories { get; } =
        [ItemInfoRenderCategory.FieldOperation, ItemInfoRenderCategory.OccultCrescent];

    public ItemOccultGoldenCofferSourceRenderer(ItemSheet itemSheet, MapSheet mapSheet, ITextureProvider textureProvider, IDalamudPluginInterface pluginInterface) : base(itemSheet, mapSheet, textureProvider, pluginInterface, ItemInfoType.OccultGoldenCoffer)
    {
    }

    public override string SingularName => "Occult Crescent (Golden Coffer)";
    public override string HelpText => "Does this item drop from a occult crescent golden coffer?";
}
