using Dalamud.Configuration;

namespace ShopItemRevealer.Configuration
{
    internal partial class ConfigurationManager : IManager, IPluginConfiguration
    {
        public int Version { get; set; } = 1;
        public bool IsConfigWindowMovable { get; set; } = true;
        public bool ShowOnlyUnobtainableItems { get; set; } = true;
        public List<uint> HideForNpcIds { get; set; } = [];
        private ShopItemRevealer Plugin { get; set; } = null!;

        public void Initialize(ShopItemRevealer plugin) {
            Plugin = plugin;
            Save();
        }
        public void Save()
        {
            PluginInterface.SavePluginConfig(this);
        }
        public void Dispose()
        {
            Save();
        }
    }
}
