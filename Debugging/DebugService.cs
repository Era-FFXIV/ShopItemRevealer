using Dalamud.Game.Command;
using Newtonsoft.Json;
using ShopItemRevealer.Game.Shops;
using System.IO;
using System.Threading.Tasks;

namespace ShopItemRevealer.Debugging
{
    /// <summary>
    /// Provides debugging services for the ShopItemRevealer plugin.
    /// </summary>
    public class DebugService : IManager
    {
        private ShopItemRevealer Plugin { get; set; } = null!;

        /// <summary>
        /// Initializes the debug service with the specified plugin.
        /// </summary>
        /// <param name="plugin">The plugin instance.</param>
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
                    _ = DumpManagerCacheAsync();
                    break;
                case "help":
                    Dalamud.Log.Information("Available commands:");
                    Dalamud.Log.Information("/shopdebug dump - Dumps the current shop cache to file.");
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Dumps the current shop cache to a file asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task DumpManagerCacheAsync()
        {
            var sh = (ShopManager)Plugin.GetManager<ShopManager>();
            var filePath = $"{Dalamud.PluginInterface.GetPluginConfigDirectory()}/shops.json";
            var json = JsonConvert.SerializeObject(sh.Shops, Formatting.Indented);
            await File.WriteAllTextAsync(filePath, json);
            Dalamud.ChatGui.Print($"Shop cache dumped to {filePath}");
        }

        /// <summary>
        /// Disposes the debug service and removes the command handler.
        /// </summary>
        public void Dispose()
        {
            Dalamud.CommandManager.RemoveHandler("/shopdebug");
        }
    }
}
