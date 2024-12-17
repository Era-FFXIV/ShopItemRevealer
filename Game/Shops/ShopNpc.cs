using Dalamud.Game.ClientState.Objects.Types;
using Lumina.Excel.Sheets;

namespace ShopItemRevealer.Game.Shops
{
    /// <summary>
    /// Represents a shop NPC.
    /// </summary>
    public class ShopNpc
    {
        /// <summary>
        /// Gets the ID of the NPC.
        /// </summary>
        public uint NpcId { get; private set; }

        /// <summary>
        /// Gets the name of the NPC.
        /// </summary>
        public string Name { get; private set; } = "";

        /// <summary>
        /// Gets the list of known shops for the NPC.
        /// </summary>
        public List<IShop> KnownShops { get; private set; } = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="ShopNpc"/> class with the specified NPC ID.
        /// </summary>
        /// <param name="npcId">The ID of the NPC.</param>
        public ShopNpc(uint npcId)
        {
            NpcId = npcId;
            Name = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(SheetManager.ENpcResidentSheet.GetRow(NpcId).Singular.ExtractText());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShopNpc"/> class with the specified game object.
        /// </summary>
        /// <param name="npc">The game object representing the NPC.</param>
        public ShopNpc(IGameObject npc)
        {
            NpcId = npc.DataId;
            Name = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(SheetManager.ENpcResidentSheet.GetRow(NpcId).Singular.ExtractText());
        }

        /// <summary>
        /// Gets the NPC data.
        /// </summary>
        /// <returns>The NPC data.</returns>
        public ENpcBase GetNpcData()
        {
            return Dalamud.DataManager.GetExcelSheet<ENpcBase>().GetRow(NpcId);
        }

        /// <summary>
        /// Adds a shop to the list of known shops for the NPC.
        /// </summary>
        /// <param name="shop">The shop to add.</param>
        public void AddShop(IShop shop)
        {
            KnownShops.Add(shop);
        }
    }
}
