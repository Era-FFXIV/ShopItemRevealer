
using Dalamud.Game.ClientState.Objects.Types;
using Lumina.Excel.Sheets;

namespace ShopItemRevealer.Game.Shops
{
    internal class ShopNpc
    {
        internal uint NpcId { get; private set; }
        internal string Name { get; private set; } = "";
        internal List<IShop> KnownShops { get; private set; } = [];

        internal ShopNpc(uint npcId)
        {
            NpcId = npcId;
            Name = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(SheetManager.ENpcResidentSheet.GetRow(NpcId).Singular.ExtractText());
        }
        internal ShopNpc(IGameObject npc)
        {
            NpcId = npc.BaseId;
            Name = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(SheetManager.ENpcResidentSheet.GetRow(NpcId).Singular.ExtractText());
        }
        internal ENpcBase GetNpcData()
        {
            return DataManager.GetExcelSheet<ENpcBase>().GetRow(NpcId);
        }
        internal void AddShop(IShop shop)
        {
            KnownShops.Add(shop);
        }
    }
}
