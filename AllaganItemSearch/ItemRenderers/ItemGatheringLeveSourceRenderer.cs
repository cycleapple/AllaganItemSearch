using System;
using System.Linq;
using System.Numerics;

using AllaganItemSearch.Extensions;

using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.ItemSources;
using AllaganLib.GameSheets.Sheets;

using Dalamud.Interface.Textures;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

using Dalamud.Bindings.ImGui;

using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace AllaganItemSearch.ItemRenderers;

public class ItemGatheringLeveSourceRenderer : ItemInfoRenderer<ItemGatheringLeveSource>
{
    private readonly ITextureProvider textureProvider;
    private readonly ItemSheet itemSheet;
    private readonly MapSheet mapSheet;
    private readonly ExcelSheet<EventItem> eventItemSheet;
    private readonly IDalamudPluginInterface pluginInterface;

    public ItemGatheringLeveSourceRenderer(ITextureProvider textureProvider, ItemSheet itemSheet, MapSheet mapSheet, ExcelSheet<EventItem> eventItemSheet, IDalamudPluginInterface pluginInterface)
    {
        this.textureProvider = textureProvider;
        this.itemSheet = itemSheet;
        this.mapSheet = mapSheet;
        this.eventItemSheet = eventItemSheet;
        this.pluginInterface = pluginInterface;
    }
    public override RendererType RendererType { get; } = RendererType.Source;

    public override ItemInfoType Type { get; } = ItemInfoType.GatheringLeve;

    public override string SingularName => "Gathering Leve";

    public override string PluralName => "Gathering Leves";

    public override string HelpText => "Is this item obtained from a gathering leve?";

    public override bool ShouldGroup => true;

    public override Action<ItemSource> DrawTooltip => source =>
    {
        var asSource = AsSource(source);
        var leveRow = asSource.Leve.Value;

        ImGui.TextUnformatted("理符：" + leveRow.Name.ExtractText());
        ImGui.TextUnformatted("職業：" + leveRow.ClassJobCategory.Value.Name.ExtractText());
        ImGui.TextUnformatted("經驗值獎勵：" + asSource.ExpReward);
        ImGui.TextUnformatted("理符額度消耗：" + leveRow.AllowanceCost);
        ImGui.TextUnformatted("掉落機率：" + asSource.LeveRewardItem.Value.ProbabilityPercent[asSource.RewardItemIndex] + "%");

        ImGui.TextUnformatted("獎勵物品：");
        using (ImRaii.PushIndent())
        {
            for (var itemGroupIndex = 0; itemGroupIndex < asSource.Leve.Value.LeveRewardItem.Value.LeveRewardItemGroup.Count; itemGroupIndex++)
            {
                var itemGroup = asSource.Leve.Value.LeveRewardItem.Value.LeveRewardItemGroup[itemGroupIndex];
                if (itemGroup.Value.Item.All(c => c.RowId == 0))
                {
                    continue;
                }
                ImGui.TextUnformatted("掉落機率：" + asSource.Leve.Value.LeveRewardItem.Value.ProbabilityPercent[itemGroupIndex] + "%");
                for (var index = 0; index < itemGroup.Value.Item.Count; index++)
                {
                    var itemId = itemGroup.Value.Item[index].RowId;
                    var count = itemGroup.Value.Count[index];
                    var isHQ = itemGroup.Value.IsHQ[index];
                    if (itemId == 0)
                    {
                        continue;
                    }

                    var item = this.itemSheet.GetRow(itemId);

                    ImGui.Image(
                        this.textureProvider.GetFromGameIcon(new GameIconLookup(item.Icon)).GetWrapOrEmpty().Handle,
                        new Vector2(18, 18) * ImGui.GetIO().FontGlobalScale);
                    ImGui.SameLine();
                    ImGui.TextUnformatted($"{item.NameString} x {count}");
                    if (isHQ)
                    {
                        ImGui.SameLine();
                        ImGui.Image(
                            this.textureProvider.GetPluginImageTexture(this.pluginInterface, "hq").GetWrapOrEmpty().Handle,
                            new Vector2(18, 18) * ImGui.GetIO().FontGlobalScale);
                    }
                }
            }
        }
    };

    public override Func<ItemSource, string> GetName => source =>
    {
        var asSource = AsSource(source);
        var leveRow = asSource.Leve.Value;
        return leveRow.Name.ExtractText();
    };
    public override Func<ItemSource, int> GetIcon => _ => Icons.LeveIcon;
}
