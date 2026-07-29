
using System;
using System.Linq;

using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.ItemSources;
using AllaganLib.Shared.Extensions;

using Dalamud.Game.Text;

using Dalamud.Bindings.ImGui;

namespace AllaganItemSearch.ItemRenderers;

public class ItemTripleTriadSourceRenderer : ItemInfoRenderer<ItemTripleTriadSource>
{
    public override RendererType RendererType { get; } = RendererType.Source;

    public override ItemInfoType Type { get; } = ItemInfoType.TripleTriad;

    public override string SingularName => "Triple Triad Card";

    public override string HelpText => "Is this item acquired from playing triple triad?";

    public override bool ShouldGroup => true;

    public override Action<ItemSource> DrawTooltip => (source) =>
    {
        var asSource = this.AsSource(source);

        ImGui.TextUnformatted("對戰費用：" + asSource.TripleTriadRow.Base.Fee + SeIconChar.Gil.ToIconString());
        ImGui.TextUnformatted("使用地區規則：" + (asSource.TripleTriadRow.Base.UsesRegionalRules ? "是" : "否"));
    };

    public override Func<ItemSource, string> GetName => (source) =>
    {
        return source.Item.NameString;
    };

    public override Func<ItemSource, int> GetIcon => (source) =>
    {
        return Icons.TripleTriadIcon;
    };
}
