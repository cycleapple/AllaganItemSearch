using System;
using System.Collections.Generic;
using System.Linq;

using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.ItemSources;

using Dalamud.Interface.Utility.Raii;

using Dalamud.Bindings.ImGui;

namespace AllaganItemSearch.ItemRenderers;

public class ItemDungeonChestSourceRenderer : ItemInfoRenderer<ItemDungeonChestSource>
{
    public override RendererType RendererType => RendererType.Source;
    public override ItemInfoType Type => ItemInfoType.DungeonChest;
    public override string SingularName => "Dungeon Chest";
    public override string PluralName => "Dungeon Chests";
    public override string HelpText => "Can the item appear in a dungeon chest?";
    public override bool ShouldGroup => true;
    public override IReadOnlyList<ItemInfoRenderCategory> Categories => [ItemInfoRenderCategory.Duty];

    public override Action<ItemSource> DrawTooltip => source =>
    {
        var asSource = this.AsSource(source);
        ImGui.Text("迷宮：" + asSource.ContentFinderCondition.FormattedName);
        using (ImRaii.PushIndent())
        {
            ImGui.Text(
                $"Chest {asSource.DungeonChest.ChestNo + 1} ({asSource.DungeonChest.Position.X} / {asSource.DungeonChest.Position.Y})");
        }
    };

    public override Func<ItemSource, string> GetName => source =>
    {
        var asSource = this.AsSource(source);

        return asSource.ContentFinderCondition.FormattedName;
    };

    public override Action<List<ItemSource>>? DrawTooltipGrouped => sources =>
    {
        var asSources = this.AsSource(sources);
        var groupedByDungeon = asSources.GroupBy(c => c.DungeonChest.ContentFinderCondition.RowId);
        foreach (var dungeon in groupedByDungeon)
        {
        ImGui.Text("迷宮：" + dungeon.First().ContentFinderCondition.Base.Name.ExtractText());
            using (ImRaii.PushIndent())
            {
                foreach (var chest in dungeon.OrderBy(c => c.DungeonChest.ChestNo))
                {
                    ImGui.Text(
                        $"Chest {chest.DungeonChest.ChestNo + 1} ({chest.DungeonChest.Position.X} / {chest.DungeonChest.Position.Y})");
                }
            }
        }

    };

    public override Func<ItemSource, int> GetIcon => _ => Icons.GoldChest;
}
