global using System;
using Dalamud.Plugin;
using ShopItemRevealer.Configuration;
using ShopItemRevealer.Game.Player;
using System.Reflection;

namespace ShopItemRevealer
{
    public sealed class ShopItemRevealer : IDalamudPlugin
    {
        public static string Name => "Shop Item Revealer";
        public static string PluginDataPath { get; private set; } = null!;
        internal ConfigurationManager Configuration { get; private set; } = null!;
        internal static IDalamudPluginInterface? PluginInterface;
        internal static IEnumerable<IManager> Managers { get; private set; } = null!;

        public ShopItemRevealer(IDalamudPluginInterface pluginInterface)
        {
            PluginInterface = pluginInterface;
            PluginInterface.Create<Dalamud>();
            Dalamud.Log.Debug($"{Name} v{Assembly.GetExecutingAssembly().GetName().Version} loaded.");
            PluginDataPath = Dalamud.PluginInterface.GetPluginConfigDirectory();
            Configuration = Dalamud.PluginInterface.GetPluginConfig() as ConfigurationManager ?? new ConfigurationManager();
            Managers =
            [
                Configuration,
                new Game.SheetManager(),
                new Game.GameManager(),
                new PlayerManager(),
                new UI.UIManager(),
                new Game.Shops.ShopManager(),
                new Debugging.DebugService(),
            ];
            foreach (var manager in Managers)
            {
                Dalamud.Log.Debug($"Initializing {manager.GetType().Name}...");
                manager.Initialize(this);
            }
            ReputationManager.Initialize();

        }
        public void Dispose()
        {
            foreach (var manager in Managers)
            {
                manager?.Dispose();
            }
            Configuration.Save();
        }

        internal IManager GetManager<T>() where T : IManager
        {
            foreach (var manager in Managers)
            {
                if (manager is T t)
                {
                    return t;
                }
            }
            throw new InvalidOperationException($"Manager of type {typeof(T).Name} not found.");
        }
    }
    internal interface IManager
    {
        public void Initialize(ShopItemRevealer plugin);
        public void Dispose();
    }
}