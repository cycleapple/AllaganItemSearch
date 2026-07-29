using System;
using System.Collections.Generic;

using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Extensions;
using AllaganLib.GameSheets.ItemSources;
using AllaganLib.GameSheets.Sheets;

using Dalamud.Interface.Utility.Raii;

using Dalamud.Bindings.ImGui;

using LuminaSupplemental.Excel.Model;

namespace AllaganItemSearch.ItemRenderers;

public class ItemMonsterDropSourceRenderer : ItemInfoRenderer<ItemMonsterDropSource>
{
    private readonly TerritoryTypeSheet territoryTypeSheet;
    private readonly MapSheet mapSheet;
    private readonly BNpcNameSheet bnpcNameSheet;

    public ItemMonsterDropSourceRenderer(TerritoryTypeSheet territoryTypeSheet, MapSheet mapSheet,
        BNpcNameSheet bnpcNameSheet)
    {
        this.territoryTypeSheet = territoryTypeSheet;
        this.mapSheet = mapSheet;
        this.bnpcNameSheet = bnpcNameSheet;
    }

    public override RendererType RendererType => RendererType.Source;
    public override ItemInfoType Type => ItemInfoType.Monster;
    public override string SingularName => "Monster Drop";
    public override string PluralName => "Monster Drops";
    public override string HelpText => "Is the item dropped from monsters?";
    public override bool ShouldGroup => true;

    public override Action<ItemSource> DrawTooltip => source =>
    {
        var asSource = this.AsSource(source);
        ImGui.Text("怪物：" + asSource.MobDrop.BNpcName.Value.Singular.ExtractText().ToTitleCase());

        ImGui.Text("地點：");
        using (ImRaii.PushIndent())
        {
            foreach (var spawnPosition in asSource.BNpcName.MobSpawnPositions)
            {
                var map = this.territoryTypeSheet.GetRowOrDefault(spawnPosition.TerritoryType.RowId)?.Map;
                if (map != null)
                {
                    ImGui.Text($"{map.FormattedName} - {spawnPosition.Position.X} / {spawnPosition.Position.Y}");
                }
            }
        }
    };

    public override Func<ItemSource, string> GetName => source =>
    {
        var asSource = this.AsSource(source);

        return asSource.MobDrop.BNpcName.Value.Singular.ExtractText().ToTitleCase();
    };

    public override Action<List<ItemSource>>? DrawTooltipGrouped => sources =>
    {
        var asSources = this.AsSource(sources);
        Dictionary<uint, HashSet<MobSpawnPosition>> positionsGroupedByNpcId = new();
        foreach (var asSource in asSources)
        {
            foreach (var position in asSource.BNpcName.MobSpawnPositions)
            {
                if (position.TerritoryType.IsValid && position.TerritoryType.ValueNullable?.Map.ValueNullable != null)
                {
                    positionsGroupedByNpcId.TryAdd(position.TerritoryType.Value.Map.RowId, new());
                    positionsGroupedByNpcId[position.TerritoryType.Value.Map.RowId].Add(position);
                }

            }
        }

        foreach (var npcGroup in positionsGroupedByNpcId)
        {
            ImGui.Text("怪物：" + this.bnpcNameSheet.GetRow(npcGroup.Key).Base.Singular.ExtractText().ToTitleCase());
            ImGui.Text("地點：");
            using (ImRaii.PushIndent())
            {
                foreach (var mobSpawnPosition in npcGroup.Value)
                {
                    ImGui.Text(
                        $"{mobSpawnPosition.BNpcName.Value.Singular.ExtractText().ToTitleCase()} - {mobSpawnPosition.Position.X} / {mobSpawnPosition.Position.Y}");
                }
            }
        }

    };

    public override Func<ItemSource, int> GetIcon => _ => Icons.MobIcon;
}
