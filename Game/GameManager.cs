using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Plugin.Services;
using ShopItemRevealer.Game.Addons;
using ShopItemRevealer.Game.Shops;

namespace ShopItemRevealer.Game
{
    internal partial class GameManager : IManager
    {
        private ShopItemRevealer Plugin { get; set; } = null!;
        public AddonShopExchangeCurrency AddonShopExchangeCurrency { get; private set; } = null!;
        public AddonFateProgress AddonFateProgress { get; private set; } = null!;
        public uint LastTarget { get; private set; }
        public void Dispose()
        {
            Framework.Update -= OnFrameworkUpdate;
            AddonShopExchangeCurrency.Dispose();
            AddonFateProgress.Dispose();
        }

        public void Initialize(ShopItemRevealer plugin)
        {
            Plugin = plugin;
            AddonShopExchangeCurrency = new AddonShopExchangeCurrency(plugin);
            AddonFateProgress = new AddonFateProgress(plugin);
            Framework.Update += OnFrameworkUpdate;
        }

        private void OnFrameworkUpdate(IFramework framework)
        {
            var target = Target.Target != null ? Target.Target.BaseId : 0;
            if (target != LastTarget)
            {
                LastTarget = target;
                if (target != 0)
                {
                    HandleNpcTargeted(Target.Target!);
                }
            }
        }
        private void HandleNpcTargeted(IGameObject npc)
        {
            if (npc.ObjectKind != ObjectKind.EventNpc) return;
            Log.Debug("Npc Targeted: " + npc.Name);
            var sm = (ShopManager)Plugin.GetManager<ShopManager>();
            sm.ShopScanner(new ShopNpc(npc));
        }
    }
}
