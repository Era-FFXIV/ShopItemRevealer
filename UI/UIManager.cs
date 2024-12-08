using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.Addon.Lifecycle;
using FFXIVClientStructs.FFXIV.Client.UI;
using ShopItemRevealer.Game.Player;
using ShopItemRevealer.UI.Commands;
using ShopItemRevealer.UI.Windows;
using System.Diagnostics;

namespace ShopItemRevealer.UI
{
    internal class UIManager : IManager
    {
        private ShopItemRevealer Plugin { get; set; } = null!;
        internal WindowManager WindowManager { get; private set; } = null!;
        internal CommandManager CommandManager { get; private set; } = null!;
        public void Dispose()
        {
            WindowManager.Dispose();
            CommandManager.Dispose();
        }
        public void Initialize(ShopItemRevealer plugin)
        {
            Plugin = plugin;
            WindowManager = new WindowManager();
            WindowManager.Initialize(plugin);
            CommandManager = new CommandManager();
            CommandManager.Initialize(plugin);
        }
        public void OnAddonOpen(AddonEvent addonEvent, AddonArgs args)
        {
            Dalamud.Log.Debug("Addon Opened");
            if (Plugin.Configuration.HideForNpcIds.Contains(Dalamud.Target.Target?.DataId ?? 0))
            {
                Dalamud.Log.Debug("Configured to hide by default, not opening.");
                return;
            }
            WindowManager.MainWindow.HandleAddonOpen();
        }
        public void OnAddonClose(AddonEvent addonEvent, AddonArgs args)
        {
            WindowManager.MainWindow.Close();
        }
        public static unsafe bool OpenAchievementWindow()
        {
            if (UIModule.Instance()->IsMainCommandUnlocked(6))
            {
                UIModule.Instance()->ExecuteMainCommand(6);
                return true;
            }
            else
            {
                return false;
            }
        }
        public void OpenMainWindow()
        {
            WindowManager.MainWindow.Open();
        }
        public static void OpenUrl(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            catch (Exception e)
            {
                Dalamud.Log.Error($"Could not open URL:\n{e.Message}");
            }
        }
        public static string GarlandToolsItemAddress(uint itemId) => $"https://www.garlandtools.org/db/#item/{itemId}";
        public static string WikiQuestAddress(Quest quest) => $"https://ffxiv.gamerescape.com/wiki/{quest.Name.Replace(" ", "_")}";
        public static string TribalWikiAddress(BeastTribe beastTribe) => $"https://ffxiv.consolegameswiki.com/wiki/{beastTribe.Name.Replace(" ", "_") + "_Daily_Quests"}";
        
    }
}
