namespace ShopItemRevealer.Game.Shops
{
    /// <summary>
    /// Represents the type of shop.
    /// </summary>
    public enum ShopType
    {
        SpecialShop,
        GilShop,
        SatisfactionShop
    }

    /// <summary>
    /// Represents a shop interface.
    /// </summary>
    public interface IShop
    {
        /// <summary>
        /// Gets the ID of the shop.
        /// </summary>
        uint ShopId { get; }

        /// <summary>
        /// Gets the type of the shop.
        /// </summary>
        ShopType ShopType { get; }

        /// <summary>
        /// Gets the list of shop items.
        /// </summary>
        List<ShopItem> ShopItems { get; }

        /// <summary>
        /// Gets the NPC associated with the shop.
        /// </summary>
        ShopNpc ShopNpc { get; }

        /// <summary>
        /// Gets a shop item by its ID.
        /// </summary>
        /// <param name="id">The ID of the shop item.</param>
        /// <returns>The shop item with the specified ID.</returns>
        ShopItem? GetShopItem(uint id);

        /// <summary>
        /// Gets a shop item by its name.
        /// </summary>
        /// <param name="name">The name of the shop item.</param>
        /// <returns>The shop item with the specified name.</returns>
        ShopItem? GetShopItem(string name);
    }
}
