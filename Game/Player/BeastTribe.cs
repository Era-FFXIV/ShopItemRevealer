using System.Globalization;

namespace ShopItemRevealer.Game.Player
{
    internal class BeastTribe : IGameInfo
    {
        
        public string Name { get; init; }
        public uint Id { get; init; }
        public uint MaxRank { get; init; }
        public string Expansion { get; init; } = "None";
        public BeastTribe(uint id)
        {
            var info = SheetManager.BeastTribeSheet.GetRow(id);
            Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(info.Name.ExtractText());
            if (Name == "")
            {
                Name = "None";
            }
            Id = id;
            MaxRank = info.MaxRank;
            Expansion = info.Expansion.Value.Name.ExtractText();
        }
    }
}
