using Dalamud.Game.Command;

namespace ShopItemRevealer.UI.Commands
{
    /// <summary>
    /// Manages the commands for the ShopItemRevealer plugin.
    /// </summary>
    public class CommandManager : IManager
    {
        private ShopItemRevealer Plugin { get; set; } = null!;

        /// <summary>
        /// Initializes the command manager with the specified plugin.
        /// </summary>
        /// <param name="plugin">The plugin instance.</param>
        public void Initialize(ShopItemRevealer plugin)
        {
            Plugin = plugin;
            Dalamud.CommandManager.AddHandler("/shopitemrevealer", new CommandInfo(OnCommand)
            {
                ShowInHelp = false, // alias for /shopitems
                HelpMessage = "Opens the Shop Item Revealer window."
            });
            Dalamud.CommandManager.AddHandler("/shopitems", new CommandInfo(OnCommand)
            {
                HelpMessage = "Opens the Shop Item Revealer window.",
            });
        }

        /// <summary>
        /// Disposes the command manager and removes the command handlers.
        /// </summary>
        public void Dispose()
        {
            Dalamud.CommandManager.RemoveHandler("/shopitemrevealer");
            Dalamud.CommandManager.RemoveHandler("/shopitems");
        }

        private void OnCommand(string command, string args)
        {
            if (command == "/shopitemrevealer" || command == "/shopitems")
            {
                if (Dalamud.Target.Target == null)
                {
                    Dalamud.ChatGui.PrintError("No target selected.");
                    return;
                }
                var ui = (UIManager)Plugin.GetManager<UIManager>();
                ui.OpenMainWindow();
            }
        }
    }
}
