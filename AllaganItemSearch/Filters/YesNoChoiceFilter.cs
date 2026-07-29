using System;
using System.Collections.Generic;
using System.Linq;

using AllaganItemSearch.ItemRenderers;
using Loc = AllaganItemSearch.Localization.Localization;

using AllaganLib.GameSheets.Sheets.Rows;
using AllaganLib.Interface.FormFields;
using AllaganLib.Interface.Services;

namespace AllaganItemSearch.Filters;

public class YesNoChoiceFilter : ChoiceFormField<YesNoChoice, FilterState>, IItemFilter
{
    private readonly string key;
    private readonly string name;
    private readonly string helpText;
    private readonly Func<ItemRow, bool> transformer;

    public delegate YesNoChoiceFilter Factory(string key, string name, string helpText, Func<ItemRow, bool> transformer);

    public YesNoChoiceFilter(string key, string name, string helpText, Func<ItemRow, bool> transformer, ImGuiService imGuiService)
        : base(imGuiService)
    {
        this.key = key;
        this.name = name;
        this.helpText = helpText;
        this.transformer = transformer;
    }

    public IReadOnlyList<ItemInfoRenderCategory>? Categories { get; set; }

    public RendererType? RendererType { get; set; }

    public override YesNoChoice DefaultValue { get; set; } = YesNoChoice.NA;

    public override string Key
    {
        get => this.key;
        set { }
    }

    public override string Name
    {
        get => this.name;
        set { }
    }

    public override string HelpText
    {
        get => this.helpText;
        set { }
    }

    public override string Version => "1.0";

    public Func<ItemRow, bool> Transformer => this.transformer;

    public bool Match(FilterState filterState, ItemRow row)
    {
        var itemRowValue = this.Transformer.Invoke(row);
        var currentValue = this.CurrentValue(filterState);
        return currentValue switch
        {
            YesNoChoice.NA => true,
            YesNoChoice.No => !itemRowValue,
            _ => itemRowValue,
        };
    }

    public IEnumerable<ItemRow> Match(FilterState filterState, IEnumerable<ItemRow> itemRows)
    {
        var currentValue = this.CurrentValue(filterState);
        return itemRows.Where(
            row =>
            {
                var itemRowValue = this.Transformer.Invoke(row);
                return currentValue switch
                {
                    YesNoChoice.NA => true,
                    YesNoChoice.No => !itemRowValue,
                    _ => itemRowValue,
                };
            });
    }

    public override Dictionary<YesNoChoice, string> Choices => new Dictionary<YesNoChoice, string>
    {
        [YesNoChoice.NA] = Loc.T("Not Set"),
        [YesNoChoice.No] = Loc.T("No"),
        [YesNoChoice.Yes] = Loc.T("Yes"),
    };
}
