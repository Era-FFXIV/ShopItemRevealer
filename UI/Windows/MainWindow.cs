using System.Numerics;
using Dalamud.Interface.Windowing;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using ShopItemRevealer.Configuration;
using ShopItemRevealer.Game;
using ShopItemRevealer.Game.Shops;
using Dalamud.Game.Text.SeStringHandling;
using ShopItemRevealer.Game.Player;
using Newtonsoft.Json;


namespace ShopItemRevealer.UI.Windows
{
    internal class MainWindow: Window, IDisposable
    {
        public ShopItemRevealer Plugin { get; private set; }
        public readonly uint MinSize = 400;
        private List<ShopItem> Items { get; set; } = [];
        private List<ShopItem> UnobtainableItems { get; set; } = [];
        private Dictionary<ShopItem, int> QuestsNeeded { get; set; } = [];
        private bool IsGemstoneTrader { get; set; } = false;
        private uint SelectedNpcId { get; set; } = 0;
        private bool ShowOnlyUnobtainableItems { get; set; } = false;
        private List<uint> HideForNpcIds { get; set; } = [];

        public MainWindow(string name, ShopItemRevealer plugin) : base(name, ImGuiWindowFlags.NoCollapse)
        {
            Plugin = plugin;
        }
        public void HandleAddonOpen()
        {
            Open(true);
        }
        public void HandleMainUiOpen()
        {
            Open();
        }
        public void Open(bool isAddonTrigger = false)
        {
            Dalamud.Log.Debug("MainWindow Open Call");
            var gm = (GameManager)Plugin.GetManager<GameManager>();
            if (!IsOpen)
            {
                var sm = (ShopManager)Plugin.GetManager<ShopManager>();
                var cm = (ConfigurationManager)Plugin.GetManager<ConfigurationManager>();
                var npc = sm.Npcs.FirstOrDefault(n => n.NpcId == gm.LastTarget);
                if (npc == null)
                {
                    Dalamud.ChatGui.PrintError("No supported shops found for this NPC.");
                    return;
                }
                Items = sm.GetShopItems(npc);
                ReputationManager.ScanAllReputations();
                if (Items.Count == 0)
                {
                    Dalamud.Log.Debug("No items found for this NPC.");
                    return;
                }
                UnobtainableItems = Items.Where(i => i.IsUnobtainable).ToList();
                Dalamud.Log.Verbose($"[DisplayPrep] Unobtainable Items: {UnobtainableItems.Count} - {JsonConvert.SerializeObject(UnobtainableItems)}");
                SelectedNpcId = npc.NpcId;
                ShowOnlyUnobtainableItems = cm.ShowOnlyUnobtainableItems;
                HideForNpcIds = cm.HideForNpcIds;
                if (SheetManager.ENpcResidentSheet.TryGetRow(npc.NpcId, out var row))
                {
                    if (row.Title.ExtractText() == "Gemstone Trader") IsGemstoneTrader = true;
                }
                if (ShowOnlyUnobtainableItems && UnobtainableItems.Count == 0 && isAddonTrigger) { 
                    Dalamud.Log.Debug("No unrevealed items and ShowOnlyUnobtainableItems is set.");
                } else
                {
                    Toggle();
                }
            } else
            {
                Dalamud.Log.Debug("MainWindow Already Open");
            }
        }
        public void Close()
        {
            if (IsOpen)
            {
                Toggle();
            }
        }
        public override void PreDraw()
        {
            var minSize = new Vector2(MinSize, 17 * ImGui.GetTextLineHeightWithSpacing());
            var maxSize = new Vector2(MinSize * 4, ImGui.GetIO().DisplaySize.Y * 15 / 16);
            ImGui.SetNextWindowSizeConstraints(minSize, maxSize);
            Flags |= ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse;
        }
        
        public override void OnOpen()
        {
            Dalamud.Log.Debug("MainWindow Opened");
        }
        public override void OnClose()
        {
            var cm = (ConfigurationManager)Plugin.GetManager<ConfigurationManager>();
            cm.ShowOnlyUnobtainableItems = ShowOnlyUnobtainableItems;
            cm.HideForNpcIds = HideForNpcIds;
            cm.Save();
            Items.Clear();
            UnobtainableItems.Clear();
        }
        public override void Draw()
        {
            DrawHeader();
            if ((ShowOnlyUnobtainableItems && UnobtainableItems.Count == 0) || Items.Count == 0)
            {
                ImGui.Text("No unrevealed items.");
            }
            else if (!PlayerManager.HasFateRanksInitialized)
            {
                ImGui.TextUnformatted("Fate ranks not initialized. Please open the Shared FATEs window to populate.");
                if (ImGui.Button("Open Shared FATEs"))
                {
                    UIManager.OpenSharedFateWindow();
                }
            }
            else {
                DrawItemTable(ShowOnlyUnobtainableItems ? UnobtainableItems : Items);
                ImGui.Separator();
                ImGui.TextUnformatted("Right-click an item for more options.");
                if (IsGemstoneTrader)
                {
                    ImGui.SameLine();
                    ImGui.TextUnformatted("If the FATE rank is not showing or is incorrect, re-open the Shared FATEs window to update.");
                }
            }
        }
        private void DrawHeader()
        {
            ImGuiUtil.Checkbox("Show Only Unrevealed Items", "Show Only Unrevealed Items", ShowOnlyUnobtainableItems, (value) =>
            {
                ShowOnlyUnobtainableItems = value;
                SortDirty = true;
            });
            ImGui.SameLine();
            var hideForThisNpc = HideForNpcIds.Contains(SelectedNpcId);
            ImGuiUtil.Checkbox("Hide Window For This NPC", "Don't Show for This NPC", hideForThisNpc, (value) =>
            {
                if (value) HideForNpcIds.Add(SelectedNpcId);
                else HideForNpcIds.Remove(SelectedNpcId);
            }, ImGuiHoveredFlags.None);
            ImGuiUtil.HelpMarker("If you don't want to see the items for this NPC, check this box. You can still use /shopitems to open this window.");
            if (UnobtainableItems.Any(i => i.IsUnobtainable && i.Requirements.Any(r => r.ReasonType == LockedReasonType.Achievement)))
            {
                if (ImGui.Button("Open Achievements"))
                {
                    UIManager.OpenAchievementWindow();
                    ImGuiUtil.HelpMarker("Achievement window needs to be open to refresh obtained achievement data.");
                }
            }
            ImGui.Separator();
        }
        private float itemNameColumnWidth;
        private bool SortDirty { get; set; } = false;
        private List<ShopItem> SortedItems { get; set; } = [];

        private void DrawItemTable(List<ShopItem> itemList)
        {
            if (SortedItems.Count != itemList.Count)
            {
                SortedItems = [.. itemList.OrderBy(i => i.ItemName)];
            }
            itemNameColumnWidth = (itemList.Max(i => WindowManager.TextWidth(i.ItemName)) + WindowManager.ItemSpacing.X + WindowManager.LineIconSize.X) / WindowManager.Scale;
            ImGui.BeginTable("Item Table", 3, ImGuiTableFlags.Resizable | ImGuiTableFlags.Reorderable | ImGuiTableFlags.Sortable | ImGuiTableFlags.ScrollY, new Vector2(0, -ImGui.GetFrameHeightWithSpacing()));
            ImGui.TableSetupScrollFreeze(0, 1);
            ImGui.TableSetupColumn("Item", ImGuiTableColumnFlags.WidthFixed | ImGuiTableColumnFlags.DefaultSort | ImGuiTableColumnFlags.PreferSortAscending, itemNameColumnWidth);
            ImGui.TableSetupColumn("Cost", ImGuiTableColumnFlags.WidthFixed | ImGuiTableColumnFlags.PreferSortDescending, ImGui.CalcTextSize("999999").X);
            ImGui.TableSetupColumn("Requirements", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableHeadersRow();
            if (ImGui.TableGetSortSpecs().SpecsDirty || SortDirty)
            {
                var spec = ImGui.TableGetSortSpecs().Specs;
                if (spec.ColumnIndex == 0)
                {
                    SortedItems = spec.SortDirection == ImGuiSortDirection.Ascending ? [.. SortedItems.OrderBy(i => i.ItemName)] : [.. SortedItems.OrderByDescending(i => i.ItemName)];
                }
                else if (spec.ColumnIndex == 1)
                {
                    SortedItems = spec.SortDirection == ImGuiSortDirection.Ascending ? [.. SortedItems.OrderBy(i => i.Price)] : [.. SortedItems.OrderByDescending(i => i.Price)];
                }
                ImGui.TableGetSortSpecs().SpecsDirty = false;
                SortDirty = false;
            }
            foreach (var item in SortedItems)
            {
                if (item.IsUnobtainable || (!ShowOnlyUnobtainableItems && !item.IsUnobtainable))
                {
                    DrawRow(item);
                }
            }
            ImGui.EndTable();
        }
        private void DrawRow(ShopItem item)
        {
            ImGui.TableNextColumn();
            using var id = ImRaii.PushId(item.GetHashCode());
            // item
            using (ImRaii.ItemWidth(itemNameColumnWidth))
            {
                if (item.GetItemIcon() != null) ImGui.Image(item.GetItemIcon().Handle, WindowManager.IconSize);
                else ImGui.Dummy(WindowManager.LineIconSize);
                ImGui.SameLine();
                ImGui.AlignTextToFramePadding();
                ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 4.0f * WindowManager.Scale);
                var selected = ImGui.Selectable(item.ItemName, false, ImGuiSelectableFlags.None);
                CreateContextMenu(item);
                if (selected) Dalamud.ChatGui.Print(SeString.CreateItemLink(item.ItemId));
            }
            ImGui.TableNextColumn();
            // cost
            using (var _ = ImRaii.ItemWidth(ImGui.CalcTextSize("999999").X))
            {
                ImGui.TextUnformatted(item.Price.ToString());
            }
            ImGui.TableNextColumn();
            // requirements
            foreach (var line in item.Requirements)
            {
                if (ImGui.IsItemHovered() && line.NeededQuests() > 0) ImGuiUtil.HoverTooltip($"Quests needed to unlock: {line.NeededQuests()}", ImGuiHoveredFlags.None);
                if (ImGui.IsItemHovered() && line.RequirementObject is BeastTribeItem tribe && tribe.RequiredReputation == 0) ImGuiUtil.HoverTooltip($"Requires max rank in all {tribe.Quest!.Expansion} beast tribes.", ImGuiHoveredFlags.None);
                using var _ = ImRaii.PushIndent(1, (line.ReasonType == LockedReasonType.BeastTribe));
                ImGui.TextWrapped(line.ToString() + "\n");
            }
        }
        private static void CreateContextMenu(ShopItem item)
        {
            var locks = item.Requirements.ToDictionary(r => r.ReasonType, r => r.RequirementObject);
            if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
            {
                ImGui.OpenPopup(item.ItemName);
            }
            using var popup = ImRaii.Popup(item.ItemName);
            if (!popup) return;
            if (ImGui.Selectable("Copy Item Name"))
            {
                ImGui.SetClipboardText(item.ItemName);
            }
            if (ImGui.Selectable("Find Item In Garland Tools"))
            {
                UIManager.OpenUrl(UIManager.GarlandToolsItemAddress(item.ItemId));
            }
            if (locks.ContainsKey(LockedReasonType.Quest))
            {
                if (ImGui.Selectable("Search Quest in Wiki"))
                {
                    UIManager.OpenUrl(UIManager.WikiQuestAddress((Quest)locks[LockedReasonType.Quest]));
                }
                if (ImGui.Selectable("Copy Quest Name"))
                {
                    ImGui.SetClipboardText(locks[LockedReasonType.Quest].Name);
                }
            }
            if (locks.ContainsKey(LockedReasonType.BeastTribe))
            {
                if (ImGui.Selectable("Search Beast Tribe in Wiki"))
                {
                    UIManager.OpenUrl(UIManager.TribalWikiAddress((BeastTribe)locks[LockedReasonType.BeastTribe]));
                }
                if (ImGui.Selectable("Copy Beast Tribe Name"))
                {
                    ImGui.SetClipboardText(locks[LockedReasonType.BeastTribe].Name);
                }
            }
        }

        public void Dispose()
        {
            
        }
    }
}
