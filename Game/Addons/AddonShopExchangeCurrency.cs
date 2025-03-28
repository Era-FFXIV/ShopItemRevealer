﻿using Dalamud.Game.Addon.Lifecycle;
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
            Dalamud.AddonLifecycle.RegisterListener(AddonEvent.PostSetup, ["ShopExchangeCurrency"], OnPostSetup);
            Dalamud.AddonLifecycle.RegisterListener(AddonEvent.PreFinalize, ["ShopExchangeCurrency"], OnFinalize);
            Dalamud.AddonLifecycle.RegisterListener(AddonEvent.PostDraw, ["ShopExchangeCurrency"], OnPostDraw);
            Dalamud.ContextMenu.OnMenuOpened += ContextMenuOpened;
        }

        private void OnPostDraw(AddonEvent type, AddonArgs args)
        {
            var addon = (AtkUnitBase*)args.Addon;
            var pos = new Vector2(addon->X, addon->Y);
            Position = pos;
            Width = addon->RootNode->GetWidth() * addon->Scale;
            Height = addon->RootNode->GetHeight() * addon->Scale;
        }

        private void OnFinalize(AddonEvent type, AddonArgs args)
        {
            Dalamud.Log.Debug("ShopExchangeCurrency Finalize");
            var manager = (UIManager)Plugin.GetManager<UIManager>();
            manager.OnAddonClose(type, args);
        }

        private void OnPostSetup(AddonEvent type, AddonArgs args)
        {
            Dalamud.Log.Debug("ShopExchangeCurrency PostSetup");
            var manager = (UIManager)Plugin.GetManager<UIManager>();
            manager.OnAddonOpen(type, args);
        }
        private void ContextMenuOpened(IMenuOpenedArgs args)
        {
            Dalamud.Log.Debug("ContextMenuOpened");
            if (args.AddonName == "ShopExchangeCurrency")
            {
                var ui = (UIManager)Plugin.GetManager<UIManager>();
                if (ui.WindowManager.MainWindow.IsOpen)
                {
                    return;
                }
                Dalamud.Log.Debug("ShopExchangeCurrency ContextMenuOpened");
                var target = Dalamud.Target.Target;
                if (!target!.IsValid()) return;
                var sm = (ShopManager)Plugin.GetManager<ShopManager>();
                if (sm.Shops.Any(s => s.ShopNpc.NpcId == target.DataId))
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
            Dalamud.AddonLifecycle.UnregisterListener(AddonEvent.PostSetup, ["ShopExchangeCurrency"], OnPostSetup);
            Dalamud.AddonLifecycle.UnregisterListener(AddonEvent.PreFinalize, ["ShopExchangeCurrency"], OnFinalize);
            Dalamud.AddonLifecycle.UnregisterListener(AddonEvent.PostDraw, ["ShopExchangeCurrency"], OnPostDraw);
            Position = Vector2.Zero;
            Dalamud.ContextMenu.OnMenuOpened -= ContextMenuOpened;
        }
    }
}
