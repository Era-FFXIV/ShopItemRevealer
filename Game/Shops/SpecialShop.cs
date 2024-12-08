using Newtonsoft.Json;
using ShopItemRevealer.Game.Player;

namespace ShopItemRevealer.Game.Shops
{
    internal class SpecialShop: IShop
    {
        public uint ShopId { get; init; }
        public ShopType ShopType { get; } = ShopType.SpecialShop;
        public List<ShopItem> ShopItems { get; private set; } = [];
        public ShopNpc ShopNpc { get; private set; } = null!;

        public SpecialShop(uint shopId, ShopNpc npc)
        {
            ShopId = shopId;
            ShopNpc = npc;
            var sheet = SheetManager.SpecialShopSheet.GetRow(shopId);
            if (sheet.RowId == 0)
            {
                Dalamud.Log.Error($"Unable to find shop {shopId}");
                return;
            }
            foreach (var item in sheet.Item)
            {
                var itemData = SheetManager.ItemSheet.GetRow(item.ReceiveItems[0].Item.RowId);
                var itemName = itemData.Name.ExtractText();
                if (itemName == "") continue;
                var itemCost = item.ItemCosts[0].CurrencyCost;
                List<Requirement> reqs = [];
                if (item.Quest.IsValid)
                {
                    reqs.Add(new Requirement(LockedReasonType.Quest, PlayerManager.GetQuest(item.Quest.RowId)));
                }
                if (item.AchievementUnlock.IsValid && item.AchievementUnlock.RowId > 0)
                {
                    reqs.Add(new Requirement(LockedReasonType.Achievement, PlayerManager.GetAchievement(item.AchievementUnlock.RowId)));
                }
                if (item.Quest.IsValid && item.Quest.Value.BeastTribe.IsValid)
                {
                    reqs.Add(new Requirement(LockedReasonType.BeastTribe, new BeastTribeItem(PlayerManager.GetBeastTribe(item.Quest.Value.BeastTribe.RowId), PlayerManager.GetQuest(item.Quest.RowId))));
                }
                var resolved = new ShopItem(ShopId, item.ReceiveItems[0].Item.RowId, itemName, itemCost, reqs, itemData.Icon);
                Dalamud.Log.Verbose(JsonConvert.SerializeObject(resolved));
                ShopItems.Add(resolved);
            }
        }
        public ShopItem? GetShopItem(uint id)
        {
            return ShopItems.FirstOrDefault(x => x.ItemId == id) ?? null;
        }
        public ShopItem? GetShopItem(string name)
        {
            return ShopItems.FirstOrDefault(x => x.ItemName == name) ?? null;
        }
    }
}
