namespace ShopItemRevealer.Game.Shops
{
    public enum ShopType
    {
        SpecialShop,
        GilShop,
        SatisfactionShop
    }
    internal interface IShop
    {
        public uint ShopId { get; }
        public ShopType ShopType { get; }
        List<ShopItem> ShopItems { get; }
        ShopNpc ShopNpc { get; }
        public ShopItem? GetShopItem(uint id);
        public ShopItem? GetShopItem(string name);
    }
}
