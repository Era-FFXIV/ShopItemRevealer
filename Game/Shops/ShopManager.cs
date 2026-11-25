using FFXIVClientStructs.FFXIV.Client.Game.Event;
using Lumina.Extensions;
using Newtonsoft.Json;
using System.IO;
using System.Reflection;

namespace ShopItemRevealer.Game.Shops
{
    internal class ShopManager : IManager
    {
        private ShopItemRevealer Plugin { get; set; } = null!;
        internal List<IShop> Shops { get; set; } = [];
        internal List<ShopNpc> Npcs { get; set; } = [];
        internal List<FateShopItem> FateItems { get; set; } = [];
        internal uint LastShopId { get; set; } = 0;
        public void Dispose()
        {
        }
        public void Initialize(ShopItemRevealer plugin)
        {
            Plugin = plugin;
            LoadFateItems();
        }
        public List<ShopItem> GetShopItems(ShopNpc npc)
        {
            var itemList = Shops.Where(shop => shop.ShopNpc == npc).SelectMany(shop => shop.ShopItems).ToList();
            // dedupe based on item ID
            var dedupedList = itemList.GroupBy(item => item.ItemId).Select(group => group.First()).ToList();
            return dedupedList;
        }
        public void ShopScanner(ShopNpc npc)
        {
            var npcData = SheetManager.ENpcBaseSheet.GetRow(npc.NpcId);
            Shops.Clear();
            foreach (var shop in npcData.ENpcData)
            {
                if (shop.RowId == 0) continue;
                if (shop.Is<Lumina.Excel.Sheets.SpecialShop>() || shop.Is<Lumina.Excel.Sheets.FateShop>())
                {
                    if (Shops.Any(s => s.ShopId == shop.RowId)) continue;
                    Log.Verbose($"Special Shop found: {shop.RowId}");
                    var specialShop = new SpecialShop(shop.RowId, npc);
                    Shops.Add(specialShop);
                    npc.AddShop(specialShop);
                }
                else
                {
                    Log.Verbose($"Unhandled shop type {shop.RowId}");
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
            Log.Debug("Scanned {0} ({1})'s shops.", npc.Name, npc.NpcId);
            if (!Shops.Any(Shops => Shops.ShopNpc == npc))
            {
                foreach (var shop in SheetManager.FateShopSheet)
                {
                    if (shop.RowId == npc.NpcId)
                    {
                        foreach (var sShop in shop.SpecialShop)
                        {
                            Log.Debug("Fate Shop found: {0} - Added NPC {1}", shop.RowId, npc.NpcId);
                            var fateShop = new SpecialShop(sShop.RowId, npc);
                            Shops.Add(fateShop);
                            npc.AddShop(fateShop);
                        }
                        
                    }
                }
                if (npc.KnownShops.Count == 0)
                {
                    Log.Debug("No shops found for {0} ({1}), trying fallback.", npc.Name, npc.NpcId);
                    DetectAgentShops(npc);
                }
                
            }
        }
        public Dictionary<uint, uint> FixedKnownNpcs = new Dictionary<uint, uint>()
        {
            { 1770285, 1033714 }
        };
        public unsafe void DetectAgentShops(ShopNpc npc)
        {
            var eventFramework = EventFramework.Instance();
            if (eventFramework == null) return;
            uint npcId;
            foreach (var eventHandler in eventFramework->EventHandlerModule.EventHandlerMap)
            {
                if (SheetManager.SpecialShopSheet.TryGetRow(eventHandler.Item1, out var specialShop))
                {
                    Log.Debug($"Special Shop found: {specialShop.RowId}");
                    if (FixedKnownNpcs.TryGetValue(specialShop.RowId, out uint value)) {
                        npcId = value;
                        var shop = new SpecialShop(specialShop.RowId, npc);
                        Shops.Add(shop);
                        npc.AddShop(shop);
                        return;
                    }
                    else
                    {
                        if (SheetManager.ENpcBaseSheet.TryGetRow(npc.NpcId, out var npcData))
                        {
                            npc.KnownShops.Add(new SpecialShop(specialShop.RowId, npc));
                            Shops.Add(new SpecialShop(specialShop.RowId, npc));
                            Log.Debug("Special Shop found: {0} - Added NPC {1}", specialShop.RowId, npc.NpcId);
                            return;
                        }
                    }
                    return;
                }
            }
            Log.Debug("No shops found.");
        }
        public void LoadFateItems()
        {
            var resourceName = "ShopItemRevealer.Data.FateItems.json";
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                Log.Error("Failed to load item data.");
                return;
            }
            using var reader = new StreamReader(stream);
            var json = reader.ReadToEnd();
            var items = JsonConvert.DeserializeObject<List<WikiFateItem>>(json);
            if (items == null) return;
            foreach (var item in items)
            {
                FateItems.Add(new FateShopItem(item));
            }
        }
    }
}
