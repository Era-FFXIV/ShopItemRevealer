using Dalamud.Game.Command;

namespace ShopItemRevealer.UI.Commands
{
    internal class CommandManager : IManager
    {
        private ShopItemRevealer Plugin { get; set; } = null!;
        public void Dispose()
        {
        }
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
