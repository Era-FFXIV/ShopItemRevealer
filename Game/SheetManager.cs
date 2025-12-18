using Lumina.Excel.Sheets;
using Lumina.Excel;

namespace ShopItemRevealer.Game
{
    internal class SheetManager : IManager
    {
        private ShopItemRevealer Plugin { get; init; } = null!;

        internal static ExcelSheet<SpecialShop> SpecialShopSheet { get; } = DataManager.GetExcelSheet<SpecialShop>();
        internal static ExcelSheet<ENpcBase> ENpcBaseSheet { get; } = DataManager.GetExcelSheet<ENpcBase>();
        internal static ExcelSheet<ENpcResident> ENpcResidentSheet { get; } = DataManager.GetExcelSheet<ENpcResident>();
        internal static ExcelSheet<TerritoryType> TerritoryTypeSheet { get; } = DataManager.GetExcelSheet<TerritoryType>();
        internal static ExcelSheet<Quest> QuestSheet { get; } = DataManager.GetExcelSheet<Quest>();
        internal static ExcelSheet<Item> ItemSheet { get; } = DataManager.GetExcelSheet<Item>();
        internal static ExcelSheet<Achievement> AchievementSheet { get; } = DataManager.GetExcelSheet<Achievement>();
        internal static ExcelSheet<BeastTribe> BeastTribeSheet { get; } = DataManager.GetExcelSheet<BeastTribe>();
        internal static ExcelSheet<BeastReputationRank> BeastReputationRankSheet { get; } = DataManager.GetExcelSheet<BeastReputationRank>();
        internal static ExcelSheet<PlaceName> PlaceNameSheet { get; } = DataManager.GetExcelSheet<PlaceName>();
        public void Dispose()
        {
        }
        public void Initialize(ShopItemRevealer plugin)
        {
        }
    }
}
