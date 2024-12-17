using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Plugin.Services;
using ShopItemRevealer.Game.Addons;
using ShopItemRevealer.Game.Shops;

namespace ShopItemRevealer.Game
{
    /// <summary>
    /// Manages the game-related functionality for the ShopItemRevealer plugin.
    /// </summary>
    public partial class GameManager : IManager
    {
        /// <summary>
        /// Gets the AddonShopExchangeCurrency instance.
        /// </summary>
        public AddonShopExchangeCurrency AddonShopExchangeCurrency { get; private set; } = null!;

        /// <summary>
        /// Gets the ID of the last targeted NPC.
        /// </summary>
        public uint LastTarget { get; private set; }

        private ShopItemRevealer Plugin { get; set; } = null!;

        /// <summary>
        /// Initializes the game manager with the specified plugin.
        /// </summary>
        /// <param name="plugin">The plugin instance.</param>
        public void Initialize(ShopItemRevealer plugin)
        {
            Plugin = plugin;
            AddonShopExchangeCurrency = new AddonShopExchangeCurrency(plugin);
            Dalamud.Framework.Update += OnFrameworkUpdate;
        }

        /// <summary>
        /// Disposes the game manager and unsubscribes from events.
        /// </summary>
        public void Dispose()
        {
            Dalamud.Framework.Update -= OnFrameworkUpdate;
            AddonShopExchangeCurrency.Dispose();
        }

        private void OnFrameworkUpdate(IFramework framework)
        {
            var target = Dalamud.Target.Target != null ? Dalamud.Target.Target.DataId : 0;
            if (target != LastTarget)
            {
                LastTarget = target;
                if (target != 0)
                {
                    HandleNpcTargeted(Dalamud.Target.Target!);
                }
            }
        }

        private void HandleNpcTargeted(IGameObject npc)
        {
            if (npc.ObjectKind != ObjectKind.EventNpc) return;
            Dalamud.Log.Debug("Npc Targeted: " + npc.Name);
            var sm = (ShopManager)Plugin.GetManager<ShopManager>();
            sm.ShopScanner(new ShopNpc(npc));
        }
    }
}
