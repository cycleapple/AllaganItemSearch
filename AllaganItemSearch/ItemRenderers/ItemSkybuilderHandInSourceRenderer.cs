using System;

using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.ItemSources;
using AllaganLib.GameSheets.Sheets;

using Dalamud.Interface.Utility.Raii;

using Dalamud.Bindings.ImGui;

namespace AllaganItemSearch.ItemRenderers;

public class ItemSkybuilderHandInSourceRenderer : ItemInfoRenderer<ItemSkybuilderHandInSource>
{
    private readonly GatheringItemSheet gatheringItemSheet;

    public ItemSkybuilderHandInSourceRenderer(GatheringItemSheet gatheringItemSheet)
    {
        this.gatheringItemSheet = gatheringItemSheet;
    }
    public override RendererType RendererType => RendererType.Use;
    public override ItemInfoType Type => ItemInfoType.SkybuilderHandIn;
    public override string SingularName => "Sky Builder Hand In";
    public override string HelpText => "Can the item be handed in at the firmament for skybuilders' scrip?";
    public override bool ShouldGroup => false;

    public override Action<ItemSource> DrawTooltip => source =>
    {
        var asSource = this.AsSource(source);
        var baseReward = asSource.HWDCrafterSupplyParams.BaseCollectableReward.Value;
        var midReward = asSource.HWDCrafterSupplyParams.MidCollectableReward.Value;
        var highReward = asSource.HWDCrafterSupplyParams.HighCollectableReward.Value;
        ImGui.Text("等級：" + asSource.Level);
        ImGui.Text("最高等級：" + asSource.LevelMax);

        ImGui.Text("獎勵：");
        using (ImRaii.PushIndent())
        {
        ImGui.Text("經驗值：" + baseReward.ExpReward + "/" + midReward.ExpReward + "/" + highReward.ExpReward);
        ImGui.Text("工票：" + baseReward.ScriptRewardAmount + "/" + midReward.ScriptRewardAmount + "/" +
                       highReward.ScriptRewardAmount);
        ImGui.Text("技巧點：" + baseReward.Points + "/" + midReward.Points + "/" + highReward.Points);
        }
    };

    public override Func<ItemSource, string> GetName => source =>
    {
        var asSource = this.AsSource(source);
        return asSource.Item.NameString;
    };
    public override Func<ItemSource, int> GetIcon => _ => Icons.SkybuildersScripIcon;
}
