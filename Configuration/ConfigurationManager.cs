using Dalamud.Configuration;

namespace ShopItemRevealer.Configuration
{
    /// <summary>
    /// Manages the configuration settings for the ShopItemRevealer plugin.
    /// </summary>
    public partial class ConfigurationManager : IManager, IPluginConfiguration
    {
        /// <summary>
        /// Gets or sets the version of the configuration.
        /// </summary>
        public int Version { get; set; } = 1;

        /// <summary>
        /// Gets or sets a value indicating whether the configuration window is movable.
        /// </summary>
        public bool IsConfigWindowMovable { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to show only unobtainable items.
        /// </summary>
        public bool ShowOnlyUnobtainableItems { get; set; } = true;

        /// <summary>
        /// Gets or sets the list of NPC IDs for which the window should be hidden.
        /// </summary>
        public List<uint> HideForNpcIds { get; set; } = [];

        private ShopItemRevealer Plugin { get; set; } = null!;

        /// <summary>
        /// Initializes the configuration manager with the specified plugin.
        /// </summary>
        /// <param name="plugin">The plugin instance.</param>
        public void Initialize(ShopItemRevealer plugin)
        {
            Plugin = plugin;
            Save();
        }

        /// <summary>
        /// Saves the configuration settings.
        /// </summary>
        public void Save()
        {
            Dalamud.PluginInterface.SavePluginConfig(this);
        }

        /// <summary>
        /// Disposes the configuration manager and saves the configuration settings.
        /// </summary>
        public void Dispose()
        {
            Save();
        }
    }
}
