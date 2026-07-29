using System;

using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.ItemSources;

using Dalamud.Bindings.ImGui;

namespace AllaganItemSearch.ItemRenderers;

public class ItemCompanyLeveSourceRenderer : ItemInfoRenderer<ItemCompanyLeveSource>
{
    public override RendererType RendererType { get; } = RendererType.Source;

    public override ItemInfoType Type { get; } = ItemInfoType.CompanyLeve;

    public override string SingularName => "Company Leve";

    public override string PluralName => "Company Leves";

    public override string HelpText => "Is this item obtained from a company leve?";

    public override bool ShouldGroup => true;

    public override Action<ItemSource> DrawTooltip => source =>
    {
        var asSource = this.AsSource(source);
        var leveRow = asSource.Leve.Value;
        ImGui.TextUnformatted("理符：" + leveRow.Name.ExtractText());
        ImGui.TextUnformatted("職業：" + leveRow.ClassJobCategory.Value.Name.ExtractText());
        ImGui.TextUnformatted("經驗值獎勵：" + asSource.ExpReward);
        ImGui.TextUnformatted("軍票獎勵：" + asSource.SealsRewarded);
        ImGui.TextUnformatted("理符額度消耗：" + leveRow.AllowanceCost);
    };

    public override Func<ItemSource, string> GetName => source =>
    {
        var asSource = this.AsSource(source);
        var leveRow = asSource.Leve.Value;
        return leveRow.Name.ExtractText();
    };

    public override Func<ItemSource, int> GetIcon => _ => Icons.LeveIcon;
}
