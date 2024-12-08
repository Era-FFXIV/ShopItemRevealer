using Lumina.Excel.Sheets;
using Lumina.Excel;

namespace ShopItemRevealer.Game
{
    internal class SheetManager : IManager
    {
        private ShopItemRevealer Plugin { get; init; } = null!;

        internal static ExcelSheet<SpecialShop> SpecialShopSheet { get; } = Dalamud.DataManager.GetExcelSheet<SpecialShop>();
        internal static ExcelSheet<ENpcBase> ENpcBaseSheet { get; } = Dalamud.DataManager.GetExcelSheet<ENpcBase>();
        internal static ExcelSheet<ENpcResident> ENpcResidentSheet { get; } = Dalamud.DataManager.GetExcelSheet<ENpcResident>();
        internal static ExcelSheet<TerritoryType> TerritoryTypeSheet { get; } = Dalamud.DataManager.GetExcelSheet<TerritoryType>();
        internal static ExcelSheet<Quest> QuestSheet { get; } = Dalamud.DataManager.GetExcelSheet<Quest>();
        internal static ExcelSheet<Item> ItemSheet { get; } = Dalamud.DataManager.GetExcelSheet<Item>();
        internal static ExcelSheet<Achievement> AchievementSheet { get; } = Dalamud.DataManager.GetExcelSheet<Achievement>();
        internal static ExcelSheet<BeastTribe> BeastTribeSheet { get; } = Dalamud.DataManager.GetExcelSheet<BeastTribe>();
        internal static ExcelSheet<BeastReputationRank> BeastReputationRankSheet { get; } = Dalamud.DataManager.GetExcelSheet<BeastReputationRank>();
        public void Dispose()
        {
        }
        public void Initialize(ShopItemRevealer plugin)
        {
        }
    }
}
