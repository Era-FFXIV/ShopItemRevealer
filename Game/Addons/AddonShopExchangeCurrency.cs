using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.Gui.ContextMenu;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ShopItemRevealer.Game.Shops;
using ShopItemRevealer.UI;
using System.Numerics;

namespace ShopItemRevealer.Game.Addons
{
    internal unsafe class AddonShopExchangeCurrency : IDisposable
    {
        private ShopItemRevealer Plugin { get; set; } = null!;
        public Vector2 Position { get; private set; }
        public float Width { get; private set; }
        public float Height { get; private set; }

        public AddonShopExchangeCurrency(ShopItemRevealer plugin)
        {
            Plugin = plugin;
            AddonLifecycle.RegisterListener(AddonEvent.PostSetup, ["ShopExchangeCurrency"], OnPostSetup);
            AddonLifecycle.RegisterListener(AddonEvent.PreFinalize, ["ShopExchangeCurrency"], OnFinalize);
            AddonLifecycle.RegisterListener(AddonEvent.PostDraw, ["ShopExchangeCurrency"], OnPostDraw);
            ContextMenu.OnMenuOpened += ContextMenuOpened;
        }

        private void OnPostDraw(AddonEvent type, AddonArgs args)
        {
            var gameAddon = GameGui.GetAddonByName("ShopExchangeCurrency", 1);
            var addon = (AtkUnitBase*)gameAddon.Address;
            var pos = new Vector2(addon->X, addon->Y);
            Position = pos;
            Width = addon->RootNode->GetWidth() * addon->Scale;
            Height = addon->RootNode->GetHeight() * addon->Scale;
        }

        private void OnFinalize(AddonEvent type, AddonArgs args)
        {
            Log.Debug("ShopExchangeCurrency Finalize");
            var manager = (UIManager)Plugin.GetManager<UIManager>();
            manager.OnAddonClose(type, args);
        }

        private void OnPostSetup(AddonEvent type, AddonArgs args)
        {
            Log.Debug("ShopExchangeCurrency PostSetup");
            var manager = (UIManager)Plugin.GetManager<UIManager>();
            manager.OnAddonOpen(type, args);
        }
        private void ContextMenuOpened(IMenuOpenedArgs args)
        {
            Log.Debug("ContextMenuOpened");
            if (args.AddonName == "ShopExchangeCurrency")
            {
                var ui = (UIManager)Plugin.GetManager<UIManager>();
                if (ui.WindowManager.MainWindow.IsOpen)
                {
                    return;
                }
                Log.Debug("ShopExchangeCurrency ContextMenuOpened");
                var target = Target.Target;
                if (!target!.IsValid()) return;
                var sm = (ShopManager)Plugin.GetManager<ShopManager>();
                if (sm.Shops.Any(s => s.ShopNpc.NpcId == target.BaseId))
                {
                    MenuItem item = new()
                    {
                        Name = "Open Shop Item Revealer",
                        PrefixChar = 'R',
                    };
                    item.OnClicked += clickedArgs => ContextMenuClick(clickedArgs);
                    args.AddMenuItem(item);
                }
            }
        }
        private void ContextMenuClick(IMenuItemClickedArgs args)
        {
            var manager = (UIManager)Plugin.GetManager<UIManager>();
            manager.WindowManager.MainWindow.Open();
        }

        public void Dispose()
        {
            AddonLifecycle.UnregisterListener(AddonEvent.PostSetup, ["ShopExchangeCurrency"], OnPostSetup);
            AddonLifecycle.UnregisterListener(AddonEvent.PreFinalize, ["ShopExchangeCurrency"], OnFinalize);
            AddonLifecycle.UnregisterListener(AddonEvent.PostDraw, ["ShopExchangeCurrency"], OnPostDraw);
            Position = Vector2.Zero;
            ContextMenu.OnMenuOpened -= ContextMenuOpened;
        }
    }
}
