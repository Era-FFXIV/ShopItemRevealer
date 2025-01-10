using ShopItemRevealer.Game.Player;

namespace ShopItemRevealer.Game.Shops
{
    [Serializable]
    internal class WikiFateItem(string name, int cost, int rankRequired, string zoneName, string questRequired = "")
    {
        public string Name { get; set; } = name;
        public int Cost { get; set; } = cost;
        public int RankRequired { get; set; } = rankRequired;
        public string ZoneName { get; set; } = zoneName;
        public string QuestRequired { get; set; } = questRequired;
    }
    internal class FateShopItem : IGameInfo
    {
        internal WikiFateItem Item { get; private set; }
        internal uint ItemId { get; private set; }
        internal uint TerritoryId { get; private set; }
        public uint Id => ItemId;
        public string Name => Item.Name;
        internal FateShopItem(WikiFateItem fateItem)
        {
            Item = fateItem;
            var item = SheetManager.ItemSheet.Where(x => x.Name.ExtractText() == fateItem.Name).FirstOrDefault();
            if (item.RowId == 0)
            {
                Dalamud.Log.Error($"Unable to find item {fateItem.Name}");
                return;
            }
            ItemId = item.RowId;
            var place = SheetManager.PlaceNameSheet.Where(x => x.Name.ExtractText() == fateItem.ZoneName).FirstOrDefault();
            if (place.RowId == 0)
            {
                Dalamud.Log.Error($"Unable to find place {fateItem.ZoneName}");
                return;
            }
            var territory = SheetManager.TerritoryTypeSheet.Where(x => x.PlaceName.RowId == place.RowId).FirstOrDefault();
            if (territory.RowId == 0)
            {
                Dalamud.Log.Error($"Unable to find territory {fateItem.ZoneName}");
                return;
            }
            TerritoryId = territory.RowId;
            Dalamud.Log.Verbose($"FateShopItem: {Item.Name} - ItemId: {ItemId} - TerritoryId: {TerritoryId}");
        }
    }
}
