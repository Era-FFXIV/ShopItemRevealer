using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.Addon.Lifecycle;
using FFXIVClientStructs.FFXIV.Client.UI;
using ShopItemRevealer.Game.Player;
using ShopItemRevealer.UI.Commands;
using ShopItemRevealer.UI.Windows;
using System.Diagnostics;

namespace ShopItemRevealer.UI
{
    /// <summary>
    /// Manages the UI for the ShopItemRevealer plugin.
    /// </summary>
    public class UIManager : IManager
    {
        private ShopItemRevealer Plugin { get; set; } = null!;
        internal WindowManager WindowManager { get; private set; } = null!;
        internal CommandManager CommandManager { get; private set; } = null!;

        /// <summary>
        /// Disposes the UI manager and its components.
        /// </summary>
        public void Dispose()
        {
            WindowManager.Dispose();
            CommandManager.Dispose();
        }

        /// <summary>
        /// Initializes the UI manager with the specified plugin.
        /// </summary>
        /// <param name="plugin">The plugin instance.</param>
        public void Initialize(ShopItemRevealer plugin)
        {
            Plugin = plugin;
            WindowManager = new WindowManager();
            WindowManager.Initialize(plugin);
            CommandManager = new CommandManager();
            CommandManager.Initialize(plugin);
        }

        /// <summary>
        /// Handles the event when an addon is opened.
        /// </summary>
        /// <param name="addonEvent">The addon event.</param>
        /// <param name="args">The addon arguments.</param>
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

        /// <summary>
        /// Handles the event when an addon is closed.
        /// </summary>
        /// <param name="addonEvent">The addon event.</param>
        /// <param name="args">The addon arguments.</param>
        public void OnAddonClose(AddonEvent addonEvent, AddonArgs args)
        {
            WindowManager.MainWindow.Close();
        }

        private const int AchievementWindowCommandId = 6;

        /// <summary>
        /// Opens the achievement window if the main command is unlocked.
        /// </summary>
        /// <returns><c>true</c> if the achievement window is opened; otherwise, <c>false</c>.</returns>
        public static unsafe bool OpenAchievementWindow()
        {
            if (UIModule.Instance()->IsMainCommandUnlocked(AchievementWindowCommandId))
            {
                UIModule.Instance()->ExecuteMainCommand(AchievementWindowCommandId);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Opens the main window of the plugin.
        /// </summary>
        public void OpenMainWindow()
        {
            WindowManager.MainWindow.Open();
        }

        /// <summary>
        /// Opens the specified URL in the default web browser.
        /// </summary>
        /// <param name="url">The URL to open.</param>
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

        /// <summary>
        /// Gets the Garland Tools item address for the specified item ID.
        /// </summary>
        /// <param name="itemId">The item ID.</param>
        /// <returns>The Garland Tools item address.</returns>
        public static string GarlandToolsItemAddress(uint itemId) => $"https://www.garlandtools.org/db/#item/{itemId}";

        /// <summary>
        /// Gets the wiki quest address for the specified quest.
        /// </summary>
        /// <param name="quest">The quest.</param>
        /// <returns>The wiki quest address.</returns>
        public static string WikiQuestAddress(Quest quest) => $"https://ffxiv.gamerescape.com/wiki/{quest.Name.Replace(" ", "_")}";

        /// <summary>
        /// Gets the tribal wiki address for the specified beast tribe.
        /// </summary>
        /// <param name="beastTribe">The beast tribe.</param>
        /// <returns>The tribal wiki address.</returns>
        public static string TribalWikiAddress(BeastTribe beastTribe) => $"https://ffxiv.consolegameswiki.com/wiki/{beastTribe.Name.Replace(" ", "_") + "_Daily_Quests"}";
    }
}
