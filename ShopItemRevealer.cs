global using System;
using Dalamud.Plugin;
using ShopItemRevealer.Configuration;
using ShopItemRevealer.Game.Player;

namespace ShopItemRevealer
{
    /// <summary>
    /// Represents the main class for the Shop Item Revealer plugin.
    /// </summary>
    public sealed class ShopItemRevealer : IDalamudPlugin
    {
        /// <summary>
        /// Gets the name of the plugin.
        /// </summary>
        public static string Name => "Shop Item Revealer";

        /// <summary>
        /// Gets the version of the plugin.
        /// </summary>
        public static string Version => "1.0.0";

        /// <summary>
        /// Gets the plugin data path.
        /// </summary>
        public static string PluginDataPath { get; private set; } = null!;

        /// <summary>
        /// Gets the configuration manager.
        /// </summary>
        public ConfigurationManager Configuration { get; private set; } = null!;

        /// <summary>
        /// Gets the Dalamud plugin interface.
        /// </summary>
        internal static IDalamudPluginInterface? PluginInterface;

        /// <summary>
        /// Gets the collection of managers.
        /// </summary>
        internal static IEnumerable<IManager> Managers { get; private set; } = null!;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShopItemRevealer"/> class.
        /// </summary>
        /// <param name="pluginInterface">The Dalamud plugin interface.</param>
        public ShopItemRevealer(IDalamudPluginInterface pluginInterface)
        {
            PluginInterface = pluginInterface;
            PluginInterface.Create<Dalamud>();
            Dalamud.Log.Debug($"{Name} v{Version} loaded.");
            PluginDataPath = Dalamud.PluginInterface.GetPluginConfigDirectory();
            Configuration = Dalamud.PluginInterface.GetPluginConfig() as ConfigurationManager ?? new ConfigurationManager();
            Managers = new List<IManager>
            {
                Configuration,
                new Game.SheetManager(),
                new Game.GameManager(),
                new UI.UIManager(),
                new Game.Shops.ShopManager(),
                new Debugging.DebugService(),
            };
            foreach (var manager in Managers)
            {
                Dalamud.Log.Debug($"Initializing {manager.GetType().Name}...");
                manager.Initialize(this);
            }
            ReputationManager.Initialize();
        }

        /// <summary>
        /// Disposes the plugin and its managers.
        /// </summary>
        public void Dispose()
        {
            foreach (var manager in Managers)
            {
                manager?.Dispose();
            }
            Configuration.Save();
        }

        /// <summary>
        /// Gets the manager of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the manager.</typeparam>
        /// <returns>The manager of the specified type.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the manager of the specified type is not found.</exception>
        public IManager GetManager<T>() where T : IManager
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

    /// <summary>
    /// Represents an interface for managers.
    /// </summary>
    public interface IManager
    {
        /// <summary>
        /// Initializes the manager with the specified plugin.
        /// </summary>
        /// <param name="plugin">The plugin instance.</param>
        void Initialize(ShopItemRevealer plugin);

        /// <summary>
        /// Disposes the manager.
        /// </summary>
        void Dispose();
    }
}
