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
                Log.Error($"Unable to find shop {shopId}");
                return;
            }
            foreach (var item in sheet.Item)
            {
                var itemData = SheetManager.ItemSheet.GetRow(item.ReceiveItems[0].Item.RowId);
                var itemName = itemData.Name.ExtractText();
                if (itemName == "") continue;
                var itemCost = item.ItemCosts[0].CurrencyCost;
                List<Requirement> reqs = [];
                var shopManager = (ShopManager)ShopItemRevealer.Managers.First(x => x is ShopManager);
                var fateItem = shopManager.FateItems.FirstOrDefault(x => x.ItemId == item.ReceiveItems[0].Item.RowId);
                if (fateItem != null)
                {
                    Requirement fateReq = new(LockedReasonType.FateRank, fateItem);
                    reqs.Add(fateReq);
                    if (fateReq.NeededQuestsInt > 0)
                    {
                        reqs.Add(new Requirement(LockedReasonType.Quest, PlayerManager.GetQuest((uint)fateReq.NeededQuestsInt)));
                    }
                }
                else if (item.Quest.IsValid && item.Quest.Value.RowId > 0)
                {
                    reqs.Add(new Requirement(LockedReasonType.Quest, PlayerManager.GetQuest(item.Quest.RowId)));
                }
                if (item.AchievementUnlock.IsValid && item.AchievementUnlock.RowId > 0)
                {
                    reqs.Add(new Requirement(LockedReasonType.Achievement, PlayerManager.GetAchievement(item.AchievementUnlock.RowId)));
                }
                if (item.Quest.IsValid && item.Quest.Value.BeastTribe.IsValid && item.Quest.Value.BeastTribe.RowId > 0 && fateItem == null)
                {
                    reqs.Add(new Requirement(LockedReasonType.BeastTribe, new BeastTribeItem(PlayerManager.GetBeastTribe(item.Quest.Value.BeastTribe.RowId), PlayerManager.GetQuest(item.Quest.RowId))));
                }
                var resolved = new ShopItem(ShopId, item.ReceiveItems[0].Item.RowId, itemName, itemCost, reqs, itemData.Icon);
                Log.Verbose(JsonConvert.SerializeObject(resolved));
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
