using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Web;

using AllaganItemSearch.Filters;
using AllaganItemSearch.ItemRenderers;
using AllaganItemSearch.Mediator;
using AllaganItemSearch.Services;
using AllaganItemSearch.Settings;
using Loc = AllaganItemSearch.Localization.Localization;

using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Sheets.Rows;
using AllaganLib.Shared.Extensions;

using DalaMock.Host.Mediator;
using DalaMock.Shared.Interfaces;

using Dalamud.Game.ClientState.Keys;
using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;

using Dalamud.Bindings.ImGui;

using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace AllaganItemSearch.Windows;

public class MainWindow : ExtendedWindow
{
    private readonly Configuration configuration;
    private readonly FilterState filterState;
    private readonly FilterService filterService;
    private readonly ItemInfoRenderService itemInfoRenderService;
    private readonly ITextureProvider textureProvider;
    private readonly FilterPanePositionSetting filterPanePositionSetting;
    private readonly IKeyState keyState;
    private readonly ITryOnService tryOnService;
    private readonly ChatService chatService;
    private readonly IClipboardService clipboardService;
    private readonly ClearSearchOnCloseSetting clearSearchOnCloseSetting;
    private readonly ExcelSheet<Item> itemSheet;
    private readonly ExcelSheet<ClassJob> classJobSheet;
    private readonly NumberFormatInfo gilNumberFormat;
    private readonly IFont font;
    private readonly IFileDialogManager fileDialogManager;
    private readonly ExcelSheet<World> worldSheet;
    private bool tryOn;

    private bool filterMenuOpen;
    private readonly StringFilter filterSearchField;

    public MainWindow(
        IFont font,
        MediatorService mediatorService,
        ImGuiService imGuiService,
        Configuration configuration,
        FilterState filterState,
        IPluginLog pluginLog,
        FilterService filterService,
        ItemInfoRenderService itemInfoRenderService,
        ITextureProvider textureProvider,
        FilterPanePositionSetting filterPanePositionSetting,
        IKeyState keyState,
        ITryOnService tryOnService,
        ChatService chatService,
        IClipboardService clipboardService,
        StringFilter.Factory stringFilterFactory,
        ClearSearchOnCloseSetting clearSearchOnCloseSetting)
        : base(mediatorService, imGuiService, "Allagan Item Search##AllaganItemSearch")
    {
        this.font = font;
        this.configuration = configuration;
        this.filterState = filterState;
        this.filterService = filterService;
        this.itemInfoRenderService = itemInfoRenderService;
        this.textureProvider = textureProvider;
        this.filterPanePositionSetting = filterPanePositionSetting;
        this.keyState = keyState;
        this.tryOnService = tryOnService;
        this.chatService = chatService;
        this.clipboardService = clipboardService;
        this.clearSearchOnCloseSetting = clearSearchOnCloseSetting;
        this.Flags = ImGuiWindowFlags.MenuBar;
        this.SizeCondition = ImGuiCond.FirstUseEver;
        this.Size = new Vector2(400, 400);
        this.filterSearchField = stringFilterFactory.Invoke("search", Loc.T("Filter Search:"), Loc.T("Type in the name of the filter you are looking for."),row => "");
    }

    public void DrawMenuBar()
    {
        if (ImGui.BeginMenuBar())
        {
            if (ImGui.BeginMenu(Loc.T("File")))
            {
                if (ImGui.MenuItem(Loc.T("Configuration")))
                {
                    this.MediatorService.Publish(new OpenWindowMessage(typeof(ConfigWindow)));
                }

                if (ImGui.MenuItem(Loc.T("Report a Issue")))
                {
                    "https://github.com/Critical-Impact/AllaganItemSearch".OpenBrowser();
                }

                if (ImGui.MenuItem("Ko-Fi"))
                {
                    "https://ko-fi.com/critical_impact".OpenBrowser();
                }

                if (ImGui.MenuItem(Loc.T("Close")))
                {
                    this.IsOpen = false;
                }

                ImGui.EndMenu();
            }

            if (ImGui.BeginMenu(Loc.T("View")))
            {
                var currentViewMode = this.filterPanePositionSetting.CurrentValue(this.configuration);
                ImGui.TextUnformatted(Loc.T("Filter Pane Position"));
                ImGui.Separator();
                if (ImGui.MenuItem(Loc.T("Left"), "", currentViewMode == FilterPanePosition.Left))
                {
                    this.filterPanePositionSetting.UpdateFilterConfiguration(this.configuration, FilterPanePosition.Left);
                }
                if (ImGui.MenuItem(Loc.T("Right"), "", currentViewMode == FilterPanePosition.Right))
                {
                    this.filterPanePositionSetting.UpdateFilterConfiguration(this.configuration, FilterPanePosition.Right);
                }
                if (ImGui.MenuItem(Loc.T("Top"), "", currentViewMode == FilterPanePosition.Top))
                {
                    this.filterPanePositionSetting.UpdateFilterConfiguration(this.configuration, FilterPanePosition.Top);
                }
                if (ImGui.MenuItem(Loc.T("Bottom"), "", currentViewMode == FilterPanePosition.Bottom))
                {
                    this.filterPanePositionSetting.UpdateFilterConfiguration(this.configuration, FilterPanePosition.Bottom);
                }

                ImGui.EndMenu();
            }

            ImGui.EndMenuBar();
        }
    }

    public override void Draw()
    {
        this.DrawMenuBar();
        var barHeight = ImGui.CalcTextSize(Loc.T("Try On")).Y + (ImGui.GetStyle().ItemSpacing.Y * 2);
        using (var wrapper = ImRaii.Child("wrapper", new Vector2(0, -barHeight), false, ImGuiWindowFlags.NoScrollbar))
        {
            if (wrapper)
            {
                var filterPanePosition = this.filterPanePositionSetting.CurrentValue(this.configuration);
                if (filterPanePosition == FilterPanePosition.Right)
                {
                    var width = ImGui.GetWindowContentRegionMax().X / 2;
                    using (var child = ImRaii.Child("Items", new Vector2(width, 0)))
                    {
                        if (child)
                        {
                            this.DrawItems();
                        }
                    }

                    ImGui.SameLine();
                    using (var child = ImRaii.Child("Filters", new Vector2(0, 0)))
                    {
                        if (child)
                        {
                            this.DrawFilters();
                        }
                    }
                }
                else if (filterPanePosition == FilterPanePosition.Bottom)
                {
                    var height = ImGui.GetWindowContentRegionMax().Y / 2;
                    using (var child = ImRaii.Child("Items", new Vector2(0, height)))
                    {
                        if (child)
                        {
                            this.DrawItems();
                        }
                    }

                    using (var child = ImRaii.Child("Filters", new Vector2(0, 0)))
                    {
                        if (child)
                        {
                            this.DrawFilters();
                        }
                    }
                }
                else if (filterPanePosition == FilterPanePosition.Left)
                {
                    var width = ImGui.GetWindowContentRegionMax().X / 2;

                    using (var child = ImRaii.Child("Filters", new Vector2(width, 0)))
                    {
                        if (child)
                        {
                            this.DrawFilters();
                        }
                    }

                    ImGui.SameLine();
                    using (var child = ImRaii.Child("Items", new Vector2(0, 0)))
                    {
                        if (child)
                        {
                            this.DrawItems();
                        }
                    }
                }
                else if (filterPanePosition == FilterPanePosition.Top)
                {
                    var height = ImGui.GetWindowContentRegionMax().Y / 2;

                    using (var child = ImRaii.Child("Filters", new Vector2(0, height)))
                    {
                        if (child)
                        {
                            this.DrawFilters();
                        }
                    }

                    using (var child = ImRaii.Child("Items", new Vector2(0, 0)))
                    {
                        if (child)
                        {
                            this.DrawItems();
                        }
                    }
                }
            }
        }

        using (var bottomBar = ImRaii.Child("bottomBar", new Vector2(0, 0), false, ImGuiWindowFlags.NoScrollbar))
        {
            if (bottomBar)
            {
                bool tryOn = this.tryOn;
                if (ImGui.Checkbox($"{Loc.T("Dressing Mode")}###Dressing Mode", ref tryOn))
                {
                    if (tryOn)
                    {
                        this.tryOnService.OpenFittingRoom();
                    }
                    this.tryOn = tryOn;
                }

                if (ImGui.IsItemHovered())
                {
                    using(var tooltip = ImRaii.Tooltip())
                    {
                        if (tooltip)
                        {
                            ImGui.TextUnformatted(Loc.T("Opens the dresser window and clicking an item automatically tries it on."));
                        }
                    }
                }
            }
        }
    }

    public void DrawItems()
    {
        var filteredItems = this.filterService.GetFilteredItems();
        if (filteredItems.Any())
        {
            ImGuiClip.ClippedDraw(filteredItems, this.DrawItem, ImGui.CalcTextSize(filteredItems[0].NameString).Y + 2);
        }
    }

    private void DrawItem(ItemRow item)
    {
        using var id = ImRaii.PushId(item.RowId.ToString());

        ImGui.Selectable(item.NameString);

        using (var popup = ImRaii.Popup("rMenu"))
        {
            if (popup)
            {
                ImGui.Text(item.NameString);
                ImGui.Separator();
                if (ImGui.Selectable(Loc.T("Open in Garland Tools")))
                {
                    $"https://www.garlandtools.org/db/#item/{item.GarlandToolsId}".OpenBrowser();
                }

                if (ImGui.Selectable(Loc.T("Open in Teamcraft")))
                {
                    $"https://ffxivteamcraft.com/db/en/item/{item.RowId}".OpenBrowser();
                }

                if (ImGui.Selectable(Loc.T("Open in Universalis")))
                {
                    $"https://universalis.app/market/{item.RowId}".OpenBrowser();
                }

                if (ImGui.Selectable(Loc.T("Open in Gamer Escape")))
                {
                    var name = item.NameString.Replace(' ', '_');
                    name = name.Replace('–', '-');

                    if (name.StartsWith("_")) // "level sync" icon
                        name = name.Substring(2);
                    $"https://ffxiv.gamerescape.com/wiki/{HttpUtility.UrlEncode(name)}?useskin=Vector".OpenBrowser();
                }

                if (ImGui.Selectable(Loc.T("Open in Console Games Wiki")))
                {
                    var name = item.NameString.Replace("#"," ").Replace("  ", " ").Replace(' ', '_');
                    name = name.Replace('–', '-');

                    if (name.StartsWith("_")) // "level sync" icon
                        name = name.Substring(2);
                    $"https://ffxiv.consolegameswiki.com/wiki/{HttpUtility.UrlEncode(name)}".OpenBrowser();
                }

                ImGui.Separator();
                if (ImGui.Selectable(Loc.T("Copy Name")))
                {
                    this.clipboardService.CopyToClipboard(item.NameString);
                }

                if (ImGui.Selectable(Loc.T("Link")))
                {
                    this.chatService.LinkItem(item);
                }

                if (item.CanTryOn && ImGui.Selectable(Loc.T("Try On")))
                {
                    if (item.CanTryOn)
                    {
                        this.tryOnService.TryOnItem(item);
                    }
                }

                if (ImGui.Selectable(Loc.T("More Information")))
                {
                    this.MediatorService.Publish(new OpenMoreInformationMessage(item.RowId));
                }
            }
        }
        if (ImGui.IsItemHovered())
        {
            if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
            {
                if (this.keyState[VirtualKey.CONTROL])
                {
                    this.chatService.LinkItem(item);
                }
                else if (this.keyState[VirtualKey.SHIFT])
                {
                    this.tryOnService.TryOnItem(item);
                }
                else if (this.keyState[VirtualKey.MENU] || !this.tryOn)
                {
                    this.MediatorService.Publish(new OpenMoreInformationMessage(item.RowId));
                }
                else if (this.tryOn)
                {
                    this.tryOnService.TryOnItem(item);
                }
            }

            if (ImGui.IsMouseClicked(ImGuiMouseButton.Right))
            {
                ImGui.OpenPopup("rMenu");
            }
            ImGui.SetNextWindowSizeConstraints(new Vector2(100,100), new Vector2(600,600));
            using (var tooltip = ImRaii.Tooltip())
            {
                if (tooltip)
                {
                    var availableWidth = ImGui.GetContentRegionAvail().X;
                    float imageStartX = availableWidth - 32;
                    ImGui.PushTextWrapPos(imageStartX);
                    ImGui.TextUnformatted(item.NameString);
                    ImGui.PopTextWrapPos();
                    ImGui.SameLine();
                    ImGui.SetCursorPosX(ImGui.GetCursorPosX() + ImGui.GetContentRegionAvail().X - 32);
                    ImGui.Image(this.textureProvider.GetFromGameIcon(new(item.Base.Icon)).GetWrapOrEmpty().Handle, new Vector2(32, 32));
                    ImGui.TextUnformatted(item.Base.ItemUICategory.Value.Name.ExtractText());
                    ImGui.Separator();
                    if (item.ClassJobCategory != null)
                    {
                        var classJobCategory = item.ClassJobCategory.Base.Name.ExtractText();
                        if (classJobCategory != string.Empty)
                        {
                            ImGui.TextUnformatted(classJobCategory);
                        }
                    }

                    ImGui.TextUnformatted($"{Loc.T("Item Level")} {item.Base.LevelItem.RowId}");
                    if (item.ClassJobCategory != null)
                    {
                        ImGui.TextUnformatted($"{Loc.T("Equip Level")} {item.Base.LevelEquip}");
                    }

                    ImGui.TextUnformatted(item.FormattedRarity);

                    if (item.EquipRace != CharacterRace.Any && item.EquipRace != CharacterRace.None)
                    {
                        ImGui.TextUnformatted($"{Loc.T("Only equippable by")} {Loc.T(item.EquipRace.ToString())}");
                    }

                    if (item.EquippableByGender != CharacterSex.Both && item.EquippableByGender != CharacterSex.NotApplicable)
                    {
                        ImGui.TextUnformatted($"{Loc.T("Only equippable by")} {Loc.T(item.EquippableByGender.ToString())}");
                    }

                    if (item.Base.CanBeHq)
                    {
                        ImGui.TextUnformatted(Loc.T("Can be HQ"));
                    }

                    if (item.Base.IsUnique)
                    {
                        ImGui.TextUnformatted(Loc.T("Unique"));
                    }

                    if (item.Base.IsUntradable)
                    {
                        ImGui.TextUnformatted(Loc.T("Untradable"));
                    }


                    if (item.Sources.Count > 0)
                    {
                        ImGui.NewLine();
                        ImGui.TextUnformatted(Loc.T("Available From:"));
                        ImGui.Separator();
                        ImGui.PushTextWrapPos();
                        var sources = item.Sources.Select(c => c.Type).Distinct().Select(
                                              c => this.itemInfoRenderService.GetSourceRendererByItemInfoType(c)
                                                       is { } renderer ? Loc.RendererName(renderer) : null)
                                          .Where(c => c != null).Select(c => c!);
                        ImGui.TextUnformatted(string.Join(", ", sources));
                        ImGui.PopTextWrapPos();
                    }


                    if (item.Uses.Count > 0)
                    {
                        ImGui.NewLine();
                        ImGui.TextUnformatted(Loc.T("Used In:"));
                        ImGui.Separator();
                        ImGui.PushTextWrapPos();
                        var uses = item.Uses.Select(c => c.Type).Distinct().Select(
                                              c => this.itemInfoRenderService.GetUseRendererByItemInfoType(c)
                                                       is { } renderer ? Loc.RendererName(renderer) : null)
                                          .Where(c => c != null).Select(c => c!);
                        ImGui.TextUnformatted(string.Join(", ", uses));
                        ImGui.PopTextWrapPos();
                    }

                    ImGui.Separator();
                    using (ImRaii.PushColor(ImGuiCol.Text, ImGuiColors.DalamudGrey))
                    {
                        ImGui.TextUnformatted(Loc.T("Ctrl: Link"));
                        if (item.CanTryOn)
                        {
                            ImGui.TextUnformatted(Loc.T("Shift: Try on"));
                        }

                        ImGui.TextUnformatted(Loc.T("Click/Alt: More information"));
                    }
                }
            }
        }
    }

    public void DrawFilters()
    {
        ImGui.Text(Loc.T("Filters"));
        ImGui.Separator();

        foreach (var filter in this.filterService.Filters.Where(c => this.configuration.IsFieldPinned(c.Key)))
        {
            this.DrawFilter(filter);
        }
        ImGui.Separator();
        using (var tabBar = ImRaii.TabBar("TabBar"))
        {
            if (tabBar)
            {
                using (var tabItem = ImRaii.TabItem($"{Loc.T("Basic")}###Basic"))
                {
                    if (tabItem)
                    {
                        foreach (var filter in this.filterService.Filters.Where(c => c.RendererType == null && !this.configuration.IsFieldPinned(c.Key)))
                        {
                            this.DrawFilter(filter);
                        }
                    }
                }
                using (var tabItem = ImRaii.TabItem($"{Loc.T("Sources")}###Sources"))
                {
                    if (tabItem)
                    {
                        foreach (var filter in this.filterService.Filters.Where(c => c.RendererType == RendererType.Source && !this.configuration.IsFieldPinned(c.Key)))
                        {
                            this.DrawFilter(filter);
                        }
                    }
                }
                using (var tabItem = ImRaii.TabItem($"{Loc.T("Uses")}###Uses"))
                {
                    if (tabItem)
                    {
                        foreach (var filter in this.filterService.Filters.Where(c => c.RendererType == RendererType.Use && !this.configuration.IsFieldPinned(c.Key)))
                        {
                            this.DrawFilter(filter);
                        }
                    }
                }

                using (var tabItem = ImRaii.TabItem($"{Loc.T("Search")}###Search", ImGuiTabItemFlags.NoTooltip))
                {
                    if (tabItem)
                    {
                        this.filterSearchField.Draw(this.filterState);

                        var currentValue = this.filterSearchField.CurrentValue(this.filterState).ToLower();
                        if (currentValue != string.Empty)
                        {
                            foreach (var filter in this.filterService.Filters.Where(
                                         c => c.Name.ToLower().PassesFilter(
                                             currentValue)))
                            {
                                this.DrawFilter(filter);
                            }
                        }
                    }
                }
            }
        }

    }

    private void DrawFilter(IItemFilter filter)
    {
        float spacing = ImGui.GetStyle().ItemSpacing.X;
        float buttonSize;
        using (var font = ImRaii.PushFont(this.font.IconFont))
        {
            string buttonText = FontAwesomeIcon.Eraser.ToIconString();
            Vector2 textSize = ImGui.CalcTextSize(buttonText);
            buttonSize = textSize.X + (ImGui.GetStyle().FramePadding.X * 2);
        }

        var totalWidth = ImGui.GetContentRegionAvail().X;
        var inputWidth = totalWidth - (buttonSize * 3) - (spacing * 2);
        using var pushedId = ImRaii.PushId(filter.Key);
        filter.DrawLabel(this.filterState);
        filter.DrawInput(this.filterState, (int?)inputWidth);
        ImGui.SameLine();
        ImGui.SetCursorPosX(inputWidth);
        using (var font = ImRaii.PushFont(this.font.IconFont))
        {
            using (ImRaii.PushColor(ImGuiCol.Button, new Vector4(0, 0, 0, 0)))
            {
                if (ImGui.Button(FontAwesomeIcon.Eraser.ToIconString()))
                {
                    filter.Reset(this.filterState);
                }

                font.Pop();
                if (ImGui.IsItemHovered())
                {
                    using (var tooltip = ImRaii.Tooltip())
                    {
                        if (tooltip)
                        {
                            ImGui.TextUnformatted(Loc.T("Clear"));
                        }
                    }
                }
            }
        }
        ImGui.SameLine();
        var isFieldPinned = this.configuration.IsFieldPinned(filter.Key);
        using (var color = ImRaii.PushColor(ImGuiCol.Text, ImGuiColors.DalamudGrey3, !isFieldPinned))
        {
            using (var btnColor = ImRaii.PushColor(ImGuiCol.Button, new Vector4(0, 0, 0, 0)))
            {
                using (var font = ImRaii.PushFont(this.font.IconFont))
                {
                    if (ImGui.Button(FontAwesomeIcon.Thumbtack.ToIconString()))
                    {
                        this.configuration.ToggleFieldPinning(filter.Key);
                    }
                    font.Pop();
                    color.Pop();
                    btnColor.Pop(); // Not sure why this needs to be popped too, but apparently it does ^_^
                    if (ImGui.IsItemHovered())
                    {
                        using (var tooltip = ImRaii.Tooltip())
                        {
                            if (tooltip)
                            {
                                ImGui.TextUnformatted(Loc.T(isFieldPinned ? "Unpin" : "Pin"));
                            }
                        }
                    }
                }
            }
        }

        ImGui.SameLine();
        using (var font = ImRaii.PushFont(this.font.IconFont))
        {
            using (ImRaii.PushColor(ImGuiCol.ButtonHovered, new Vector4(0, 0, 0, 0)))
            {
                using (ImRaii.PushColor(ImGuiCol.Button, new Vector4(0, 0, 0, 0)))
                {
                    if (ImGui.Button(FontAwesomeIcon.Question.ToIconString()))
                    {
                    }

                    font.Pop();
                    if (ImGui.IsItemHovered())
                    {
                        using (var tooltip = ImRaii.Tooltip())
                        {
                            if (tooltip)
                            {
                                ImGui.TextUnformatted(filter.HelpText);
                                if (filter is StringFilter)
                                {
                                    ImGui.Separator();
                                    ImGui.TextUnformatted(
                                        Loc.T("When searching the following operators can be used to compare:"));
                                    ImGui.TextUnformatted("");
                                    ImGui.TextUnformatted(Loc.T("=, for exact comparisons"));
                                    ImGui.TextUnformatted(Loc.T("!, for inequality comparisons"));
                                    ImGui.TextUnformatted(Loc.T("||, search multiple expressions using OR"));
                                    ImGui.TextUnformatted(Loc.T("&&, search multiple expressions using AND"));
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public override void OnClose()
    {
        base.OnClose();
        if (this.clearSearchOnCloseSetting.CurrentValue(this.configuration))
        {
            foreach (var filter in this.filterService.Filters)
            {
                filter.Reset(this.filterState);
            }
        }
    }
}
