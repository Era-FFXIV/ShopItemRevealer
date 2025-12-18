using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Textures;

namespace ShopItemRevealer.Game.Shops
{


    internal class ShopItem(uint shopId, uint id, string itemName, uint price, List<Requirement> requirements, ushort itemIconId)
    {
        public uint Shop { get; init; } = shopId;
        public uint ItemId { get; init; } = id;
        public uint Price { get; init; } = price;
        public List<Requirement> Requirements { get; init; } = requirements;
        public ushort ItemIconId { get; } = itemIconId;
        public string ItemName { get; init; } = itemName;
        public bool IsUnobtainable => Requirements.Any(r => !r.MeetsRequirement());
        public IDalamudTextureWrap GetItemIcon()
        {
            return TextureProvider.GetFromGameIcon(new GameIconLookup(ItemIconId)).GetWrapOrEmpty();
        }
    }
}
