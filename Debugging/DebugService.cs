using Dalamud.Game.Command;
using Newtonsoft.Json;
using ShopItemRevealer.Game.Shops;

namespace ShopItemRevealer.Debugging
{
    internal class DebugService : IManager
    {
        private ShopItemRevealer Plugin { get; set; } = null!;

        public void Initialize(ShopItemRevealer plugin)
        {
            Plugin = plugin;
            if (Dalamud.PluginInterface.IsDev || Dalamud.PluginInterface.IsTesting)
            {
                Dalamud.Log.Information("Debug Service initialized as you are using a testing version of the plugin. Additional commands are available for troubleshooting.");

                Dalamud.CommandManager.AddHandler("/shopdebug", new CommandInfo(OnCommand)
                {
                    HelpMessage = "Shop Item Revealer debug commands."
                });
            }
        }
        private void OnCommand(string command, string args)
        {
            switch (args)
            {
                case "dump":
                    DumpManagerCache();
                    break;
                case "help":
                    Dalamud.Log.Information("Available commands:");
                    Dalamud.Log.Information("/shopdebug dump - Dumps the current shop cache to file.");
                    break;
                default:
                    break;
            }
        }
        private void DumpManagerCache()
        {
            var sh = (ShopManager)Plugin.GetManager<ShopManager>();
            File.WriteAllText($"{Dalamud.PluginInterface.GetPluginConfigDirectory()}/shops.json", JsonConvert.SerializeObject(sh.Shops, Formatting.Indented));
            Dalamud.ChatGui.Print($"Shop cache dumped to {Dalamud.PluginInterface.GetPluginConfigDirectory()}/shops.json");
        }
        public void Dispose()
        {
            Dalamud.CommandManager.RemoveHandler("/shopdebug");
        }
    }
}
