using Dalamud.Game.ClientState.Objects;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using System.Runtime.InteropServices;

namespace ShopItemRevealer.Game.Shops
{
    /// <summary>
    /// Manages the shops in the game.
    /// </summary>
    public class ShopManager : IManager
    {
        private ShopItemRevealer Plugin { get; set; } = null!;

        /// <summary>
        /// Gets or sets the list of shops.
        /// </summary>
        public List<IShop> Shops { get; set; } = new();

        /// <summary>
        /// Gets or sets the list of NPCs.
        /// </summary>
        public List<ShopNpc> Npcs { get; set; } = new();

        /// <summary>
        /// Disposes the shop manager.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Initializes the shop manager with the specified plugin.
        /// </summary>
        /// <param name="plugin">The plugin instance.</param>
        public void Initialize(ShopItemRevealer plugin)
        {
            Plugin = plugin;
        }

        /// <summary>
        /// Gets the list of shop items for the specified NPC.
        /// </summary>
        /// <param name="npc">The NPC.</param>
        /// <returns>The list of shop items.</returns>
        public List<ShopItem> GetShopItems(ShopNpc npc)
        {
            return Shops.Where(shop => shop.ShopNpc == npc).SelectMany(shop => shop.ShopItems).ToList();
        }

        /// <summary>
        /// Scans the shops for the specified NPC.
        /// </summary>
        /// <param name="npc">The NPC.</param>
        public void ShopScanner(ShopNpc npc)
        {
            var npcData = SheetManager.ENpcBaseSheet.GetRow(npc.NpcId);
            Shops.Clear();
            foreach (var shop in npcData.ENpcData)
            {
                if (shop.RowId == 0) continue;
                if (shop.Is<Lumina.Excel.Sheets.SpecialShop>())
                {
                    if (Shops.Any(s => s.ShopId == shop.RowId)) continue;
                    Dalamud.Log.Debug($"Special Shop found: {shop.RowId}");
                    var specialShop = new SpecialShop(shop.RowId, npc);
                    Shops.Add(specialShop);
                    npc.AddShop(specialShop);
                }
                else
                {
                    Dalamud.Log.Debug($"Unhandled shop type {shop.RowId}");
                }
            }
            if (Npcs.Any(n => n.NpcId == npc.NpcId))
            {
                Npcs.Remove(Npcs.First(n => n.NpcId == npc.NpcId));
                Npcs.Add(npc);
            }
            else
            {
                Npcs.Add(npc);
            }
            Dalamud.Log.Debug("Scanned {0} ({1})'s shops.", npc.Name, npc.NpcId);
            if (!Shops.Any(Shops => Shops.ShopNpc == npc))
            {
                Dalamud.Log.Debug("No shops found for {0} ({1}), trying fallback.", npc.Name, npc.NpcId);
                DetectAgentShops();
            }
        }

        /// <summary>
        /// Gets or sets the dictionary of fixed known NPCs.
        /// </summary>
        public Dictionary<uint, uint> FixedKnownNpcs { get; set; } = new Dictionary<uint, uint>()
        {
            { 1770285, 1033714 }
        };

        /// <summary>
        /// Detects agent shops.
        /// </summary>
        public unsafe void DetectAgentShops()
        {
            var eventFramework = EventFramework.Instance();
            if (eventFramework == null) return;
            uint npcId;
            foreach (var eventHandler in eventFramework->EventHandlerModule.EventHandlerMap)
            {
                if (SheetManager.SpecialShopSheet.TryGetRow(eventHandler.Item1, out var specialShop))
                {
                    Dalamud.Log.Debug($"Special Shop found: {specialShop.RowId}");
                    if (FixedKnownNpcs.TryGetValue(specialShop.RowId, out uint value)) {
                        npcId = value;
                        Shops.Add(new SpecialShop(specialShop.RowId, new ShopNpc(npcId)));
                        return;
                    }
                }
            }
            Dalamud.Log.Debug("No shops found.");
        }
    }
}
