using System;

using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.ItemSources;

using Dalamud.Bindings.ImGui;

namespace AllaganItemSearch.ItemRenderers;

public class ItemGcSupplyDutySourceRenderer : ItemInfoRenderer<ItemGCSupplyDutySource>
{
    public override RendererType RendererType => RendererType.Use;
    public override ItemInfoType Type => ItemInfoType.GCDailySupply;
    public override string SingularName => "Grand Company Supply & Provisioning";
    public override string HelpText => "Can the item be handed in for 'Supply & Provisioning' at your grand company?";
    public override bool ShouldGroup => true;

    public override Action<ItemSource> DrawTooltip => source =>
    {
        var asSource = this.AsSource(source);
        var rewardRow = asSource.DailySupplyRewardRow;
        if (rewardRow != null)
        {
            var baseReward = rewardRow.Base.ExperienceSupply;
            var sealsSupply = rewardRow.Base.SealsSupply;
        ImGui.Text("等級：" + asSource.GCSupplyDutyRow.RowId);
        ImGui.Text("經驗值：" + baseReward);
        ImGui.Text("軍票：" + sealsSupply);
        }
        else
        {
            ImGui.Text("未知獎勵");
        }
    };

    public override Func<ItemSource, string> GetName => source =>
    {
        var asSource = this.AsSource(source);
        var rewardRow = asSource.DailySupplyRewardRow;
        return rewardRow != null ? asSource.GCSupplyDutyRow.RowId.ToString() : "";
    };
    public override Func<ItemSource, int> GetIcon => _ => Icons.FlameSealIcon;
}
