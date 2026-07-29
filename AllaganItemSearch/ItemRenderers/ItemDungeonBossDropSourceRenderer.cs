using System;
using System.Collections.Generic;
using System.Linq;

using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Extensions;
using AllaganLib.GameSheets.ItemSources;

using Dalamud.Interface.Utility.Raii;

using Dalamud.Bindings.ImGui;

namespace AllaganItemSearch.ItemRenderers;

public class ItemDungeonBossDropSourceRenderer : ItemInfoRenderer<ItemDungeonBossDropSource>
{
    public override RendererType RendererType => RendererType.Source;
    public override ItemInfoType Type => ItemInfoType.DungeonBossDrop;
    public override string SingularName => "Dungeon Boss Drop";
    public override string PluralName => "Dungeon Boss Drops";
    public override string HelpText => "Can the item be drop from a dungeon boss?";
    public override bool ShouldGroup => true;
    public override IReadOnlyList<ItemInfoRenderCategory> Categories => [ItemInfoRenderCategory.Duty];

    public override Action<ItemSource> DrawTooltip => source =>
    {
        var asSource = this.AsSource(source);
        ImGui.Text("迷宮：" + asSource.ContentFinderCondition.FormattedName);
        using (ImRaii.PushIndent())
        {
        ImGui.Text(asSource.BNpcName.Base.Singular.ExtractText().ToTitleCase() + "（首領 " + (asSource.DungeonBoss.FightNo + 1) + "）");
        }
    };

    public override Func<ItemSource, string> GetName => source =>
    {
        var asSource = this.AsSource(source);

        return asSource.ContentFinderCondition.FormattedName + " - " + asSource.DungeonBoss.BNpcName.Value.Singular.ExtractText();
    };

    public override Action<List<ItemSource>>? DrawTooltipGrouped => sources =>
    {
        var asSources = this.AsSource(sources);
        var groupedByDungeon = asSources.GroupBy(c => c.DungeonBoss.ContentFinderCondition.RowId);
        foreach (var dungeon in groupedByDungeon)
        {
        ImGui.Text("迷宮：" + dungeon.First().DungeonBoss.ContentFinderCondition.Value.Name.ExtractText());
            using (ImRaii.PushIndent())
            {
                foreach (var itemSource in dungeon.DistinctBy(c => c.DungeonBoss.BNpcName.RowId))
                {
        ImGui.Text(itemSource.BNpcName.Base.Singular.ExtractText().ToTitleCase() + "（首領 " + (itemSource.DungeonBoss.FightNo + 1) + "）");
                }
            }
        }

    };

    public override Func<ItemSource, int> GetIcon => _ => Icons.DutyIcon;
}
