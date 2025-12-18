using Dalamud.Game.Command;
using Newtonsoft.Json;
using ShopItemRevealer.Game.Shops;
using System.IO;

namespace ShopItemRevealer.Debugging
{
    internal class DebugService : IManager
    {
        private ShopItemRevealer Plugin { get; set; } = null!;

        public void Initialize(ShopItemRevealer plugin)
        {
            Plugin = plugin;
            if (PluginInterface.IsDev || PluginInterface.IsTesting)
            {
                Log.Information("Debug Service initialized as you are using a testing version of the plugin. Additional commands are available for troubleshooting.");

                CommandManager.AddHandler("/shopdebug", new CommandInfo(OnCommand)
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
                    Log.Information("Available commands:");
                    Log.Information("/shopdebug dump - Dumps the current shop cache to file.");
                    break;
                default:
                    break;
            }
        }
        private void DumpManagerCache()
        {
            var sh = (ShopManager)Plugin.GetManager<ShopManager>();
            File.WriteAllText($"{PluginInterface.GetPluginConfigDirectory()}/shops.json", JsonConvert.SerializeObject(sh.Shops, Formatting.Indented));
            ChatGui.Print($"Shop cache dumped to {PluginInterface.GetPluginConfigDirectory()}/shops.json");
        }
        public void Dispose()
        {
            CommandManager.RemoveHandler("/shopdebug");
        }
    }
}
