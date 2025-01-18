using ShopItemRevealer.Game.Player;

namespace ShopItemRevealer.Game.Shops
{
    [Serializable]
    internal class WikiFateItem
    {
        public string Name { get; init; }
        public int Cost { get; init; }
        public int RankRequired { get; init; }
        public string ZoneName { get; init; }
        public string QuestRequired { get; init; }
        public int ItemId { get; init; }
        public int TerritoryId { get; init; }
        public int QuestId { get; init; }
        public string Expansion { get; init; }
        public bool IsAllItems { get; init; }

        public WikiFateItem(int cost, int rankRequired, int zoneId, int questRequired, int itemId, string expansion, bool isAllItems)
        {
            Cost = cost;
            RankRequired = rankRequired;
            TerritoryId = zoneId;
            QuestId = questRequired;
            ItemId = itemId;
            Expansion = expansion;
            IsAllItems = isAllItems;

            if (SheetManager.ItemSheet.TryGetRow((uint)ItemId, out var item))
            {
                Name = item.Name.ExtractText();
            }
            else
            {
                Name = "Unknown";
                Dalamud.Log.Error($"[WikiFateItem] Item {ItemId} not found.");
            }
            if (TerritoryId != 0 && SheetManager.TerritoryTypeSheet.TryGetRow((uint)TerritoryId, out var territory))
            {
                ZoneName = territory.PlaceName.Value.Name.ExtractText();
            }
            else
            {
                ZoneName = expansion;
            }
            if (QuestId != 0 && SheetManager.QuestSheet.TryGetRow((uint)QuestId, out var quest))
            {
                QuestRequired = quest.Name.ExtractText();
            }
            else
            {
                QuestRequired = "N/A";
            }
        }
    }
    internal class FateShopItem : IGameInfo
    {
        internal WikiFateItem Item { get; private set; }
        internal uint ItemId => (uint)Item.ItemId;
        internal uint TerritoryId => (uint)Item.TerritoryId;
        public uint Id => ItemId;
        public string Name => Item.Name;
        internal FateShopItem(WikiFateItem fateItem)
        {
            Item = fateItem;
            Dalamud.Log.Verbose($"FateShopItem: {Item.Name} - ItemId: {ItemId} - TerritoryId: {TerritoryId}");
        }
    }
}
