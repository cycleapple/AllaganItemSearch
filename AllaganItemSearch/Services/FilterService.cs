using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AllaganItemSearch.Filters;
using Loc = AllaganItemSearch.Localization.Localization;

using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Service;
using AllaganLib.GameSheets.Sheets;
using AllaganLib.GameSheets.Sheets.Rows;

using Dalamud.Plugin.Services;

using Lumina.Excel.Sheets;

namespace AllaganItemSearch.Services;

public class FilterService : IDisposable
{
    private readonly StringFilter.Factory stringFilterFactory;
    private readonly YesNoChoiceFilter.Factory yesNoChoiceFactory;
    private readonly UintChoiceFilter.Factory uintChoiceFactory;
    private readonly FloatRangeFilter.Factory floatRangeFactory;
    private readonly UintRangeFilter.Factory uintRangeFactory;
    private readonly ItemInfoRenderService itemInfoRenderService;
    private readonly IDataManager dataManager;
    private readonly SheetManager sheetManager;
    private readonly FilterState filterState;
    private readonly ItemInfoCache itemInfoCache;
    private CancellationTokenSource cancellationTokenSource;

    private List<IItemFilter> filters = new();

    private List<ItemRow> itemRows = new();

    private List<ItemRow> defaultItemList = new();

    private Task<List<ItemRow>>? filterTask = null;

    public FilterService(
        StringFilter.Factory stringFilterFactory,
        YesNoChoiceFilter.Factory yesNoChoiceFactory,
        UintChoiceFilter.Factory uintChoiceFactory,
        FloatRangeFilter.Factory floatRangeFactory,
        UintRangeFilter.Factory uintRangeFactory,
        ItemInfoRenderService itemInfoRenderService,
        IDataManager dataManager,
        FilterState filterState,
        ItemSheet itemSheet,
        ItemInfoCache itemInfoCache)
    {
        this.cancellationTokenSource = new CancellationTokenSource();
        this.defaultItemList = itemSheet.Where(c => c.NameString != string.Empty).OrderBy(c => c.NameString).ToList();
        this.stringFilterFactory = stringFilterFactory;
        this.yesNoChoiceFactory = yesNoChoiceFactory;
        this.uintChoiceFactory = uintChoiceFactory;
        this.floatRangeFactory = floatRangeFactory;
        this.uintRangeFactory = uintRangeFactory;
        this.itemInfoRenderService = itemInfoRenderService;
        this.dataManager = dataManager;
        this.filterState = filterState;
        this.itemInfoCache = itemInfoCache;
        this.AddFilters();
    }

    public List<IItemFilter> Filters
    {
        get => this.filters;
    }

    private void AddFilters()
    {
        this.filters.Add(this.stringFilterFactory.Invoke("name", Loc.T("Name"), Loc.T("The name of the item"), row => row.NameString));
        this.filters.Add(this.uintChoiceFactory.Invoke("category", Loc.T("Category"), Loc.T("The category of the item"), row => row.Base.ItemUICategory.RowId, this.dataManager.GetExcelSheet<ItemUICategory>().Where(c => c.RowId != 0 && c.Name.ExtractText() != string.Empty).ToDictionary(c => c.RowId, c => c.Name.ExtractText())));
        this.filters.Add(this.uintRangeFactory.Invoke("equipLevel", Loc.T("Equip Level"), Loc.T("The equip level of the item"), row => row.Base.LevelEquip, 1, 100));
        this.filters.Add(this.uintRangeFactory.Invoke("itemLevel", Loc.T("Item Level"), Loc.T("The item's iLvl"), row => row.Base.LevelItem.RowId, 1, this.dataManager.GetExcelSheet<Item>().MaxBy(c => c.LevelItem.RowId).LevelItem.RowId));
        this.filters.Add(
            this.uintChoiceFactory.Invoke(
                "rarity",
                Loc.T("Rarity"),
                Loc.T("The rarity of the item"),
                row => row.Base.Rarity,
                new Dictionary<uint, string>()
        {
            { 1, Loc.T("Normal") },
            { 2, Loc.T("Scarce") },
            { 3, Loc.T("Artifact") },
            { 4, Loc.T("Relic") },
            { 7, Loc.T("Aetherial") },
        }));

        this.filters.Add(
            this.uintChoiceFactory.Invoke(
                "classRole",
                Loc.T("Class Role"),
                Loc.T("The class/job role"),
                row => row.ClassJobCategory?.ClassJobs.FirstOrDefault()
                          ?.Base.Role ??
                       0,
                new Dictionary<uint, string>()
        {
            { 0, Loc.T("Non-combatant") },
            { 1, Loc.T("Tank") },
            { 2, Loc.T("Melee DPS") },
            { 3, Loc.T("Ranged DPS") },
            { 4, Loc.T("Healer") },
        }));

        this.filters.Add(
            this.uintChoiceFactory.Invoke(
                "race",
                Loc.T("Race"),
                Loc.T("The race required to equip the item"),
                row => (uint)row.EquipRace,
                new Dictionary<uint, string>()
        {
            { (uint)CharacterRace.Hyur, Loc.T("Hyur") },
            { (uint)CharacterRace.Elezen, Loc.T("Elezen") },
            { (uint)CharacterRace.Hrothgar, Loc.T("Hrothgar") },
            { (uint)CharacterRace.Lalafell, Loc.T("Lalafell") },
            { (uint)CharacterRace.Miqote, Loc.T("Miqote") },
            { (uint)CharacterRace.Roegadyn, Loc.T("Roegadyn") },
            { (uint)CharacterRace.Viera, Loc.T("Viera") },
            { (uint)CharacterRace.AuRa, Loc.T("AuRa") },
        }));

        this.filters.Add(
            this.uintChoiceFactory.Invoke(
                "sex",
                Loc.T("Sex"),
                Loc.T("The sex required to equip the item"),
                row => (uint)row.EquippableByGender,
                new Dictionary<uint, string>()
        {
            { (uint)CharacterSex.Male, Loc.T("Male") },
            { (uint)CharacterSex.Female, Loc.T("Female") },
        }));

        this.filters.Add(
            this.yesNoChoiceFactory.Invoke(
                "canBeHq",
                Loc.T("Can be HQ?"),
                Loc.T("Can this item be high quality?"),
                row => row.Base.CanBeHq));

        this.filters.Add(
            this.yesNoChoiceFactory.Invoke(
                "isUnique",
                Loc.T("Is Unique?"),
                Loc.T("Is this item unique?"),
                row => row.Base.IsUnique));

        this.filters.Add(
            this.yesNoChoiceFactory.Invoke(
                "tradable",
                Loc.T("Is Tradable?"),
                Loc.T("Is this item tradable?"),
                row => !row.Base.IsUntradable));

        this.filters.Add(
            this.yesNoChoiceFactory.Invoke(
                "equippable",
                Loc.T("Is Equippable?"),
                Loc.T("Can the item be equipped?"),
                row => row.CanTryOn));

        var sourceUseFilters = new List<IItemFilter>();

        foreach (var source in Enum.GetValues<ItemInfoType>())
        {
            if (this.itemInfoRenderService.SourceRenderersByItemInfoType.ContainsKey(source))
            {
                var renderer = this.itemInfoRenderService.SourceRenderersByItemInfoType[source];
                var yesNoChoiceFilter = this.yesNoChoiceFactory.Invoke(
                    "s_" + source,
                    Loc.T("Source - ") + Loc.RendererName(renderer),
                    Loc.RendererHelp(renderer),
                    row => row.HasSourcesByType(source));
                yesNoChoiceFilter.RendererType = renderer.RendererType;
                yesNoChoiceFilter.Categories = renderer.Categories;
                sourceUseFilters.Add(
                    yesNoChoiceFilter);
            }
        }

        foreach (var source in Enum.GetValues<ItemInfoType>())
        {
            if (this.itemInfoRenderService.UseRenderersByItemInfoType.ContainsKey(source))
            {
                var renderer = this.itemInfoRenderService.UseRenderersByItemInfoType[source];
                var yesNoChoiceFilter = this.yesNoChoiceFactory.Invoke(
                    "u_" + source,
                    Loc.T("Use - ") + Loc.RendererName(renderer),
                    Loc.RendererHelp(renderer),
                    row => row.HasUsesByType(source));
                yesNoChoiceFilter.RendererType = renderer.RendererType;
                yesNoChoiceFilter.Categories = renderer.Categories;
                sourceUseFilters.Add(
                    yesNoChoiceFilter);
            }
        }

        this.filters.AddRange(sourceUseFilters.OrderBy(c => c.Name));
    }

    public List<ItemRow> GetFilteredItems()
    {
        if (this.filterState.IsDirty)
        {
            this.filterState.IsDirty = false;
            this.filterTask = (Task<List<ItemRow>>?)Task.Run(() => this.GenerateFilteredItemsList(this.cancellationTokenSource.Token), this.cancellationTokenSource.Token);
        }

        if (this.filterTask?.IsCompleted == true)
        {
            this.itemRows = this.filterTask.Result;
            this.filterTask = null;
        }

        return this.itemRows;
    }

    private List<ItemRow> GenerateFilteredItemsList(CancellationToken token)
    {
        IEnumerable<ItemRow> allItems = this.defaultItemList.OrderBy(c => c.NameString).ToList();
        foreach (var filter in this.filters)
        {
            if (token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
            }
            if (filter.HasValueSet(this.filterState))
            {
                allItems = filter.Match(this.filterState, allItems);
            }
        }

        return allItems.ToList();
    }

    public void Dispose()
    {
        this.cancellationTokenSource.Dispose();
    }
}
