using Newtonsoft.Json;
using ShopItemRevealer.Game.Player;

namespace ShopItemRevealer.Game.Shops
{
    /// <summary>
    /// Represents a special shop in the game.
    /// </summary>
    public class SpecialShop : IShop
    {
        /// <summary>
        /// Gets the ID of the shop.
        /// </summary>
        public uint ShopId { get; init; }

        /// <summary>
        /// Gets the type of the shop.
        /// </summary>
        public ShopType ShopType { get; } = ShopType.SpecialShop;

        /// <summary>
        /// Gets the list of shop items.
        /// </summary>
        public List<ShopItem> ShopItems { get; private set; } = new();

        /// <summary>
        /// Gets the NPC associated with the shop.
        /// </summary>
        public ShopNpc ShopNpc { get; private set; } = null!;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpecialShop"/> class with the specified shop ID and NPC.
        /// </summary>
        /// <param name="shopId">The ID of the shop.</param>
        /// <param name="npc">The NPC associated with the shop.</param>
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
                List<Requirement> reqs = new();
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

        /// <summary>
        /// Gets a shop item by its ID.
        /// </summary>
        /// <param name="id">The ID of the shop item.</param>
        /// <returns>The shop item with the specified ID.</returns>
        public ShopItem? GetShopItem(uint id)
        {
            return ShopItems.FirstOrDefault(x => x.ItemId == id) ?? null;
        }

        /// <summary>
        /// Gets a shop item by its name.
        /// </summary>
        /// <param name="name">The name of the shop item.</param>
        /// <returns>The shop item with the specified name.</returns>
        public ShopItem? GetShopItem(string name)
        {
            return ShopItems.FirstOrDefault(x => x.ItemName == name) ?? null;
        }
    }
}
