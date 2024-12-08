using FFXIVClientStructs.FFXIV.Client.Game;

namespace ShopItemRevealer.Game.Player
{
    internal class Quest : IGameInfo
    {
        public uint Id { get; private set; }
        public string Name { get; init; } = null!;
        public bool IsCompleted { get; init; } = false;
        public uint BeastReputationRank { get; init; } = 0;
        public string BeastReputationRankName { get; init; } = "None";
        public uint BeastReputationValue { get; init; } = 0;
        public string Expansion { get; init; } = "None";
        public override string ToString()
        {
            return $"Quest: {Name}";
        }
        public Quest(uint id)
        {
            Id = id;
            var questInfo = SheetManager.QuestSheet.GetRow(Id);
            if (questInfo.RowId == 0)
            {
                Dalamud.Log.Warning($"Unable to find quest {Id}");
                Name = "Unknown Quest";
                return;
            }
            Name = questInfo.Name.ExtractText().Replace(" ", "");
            IsCompleted = QuestManager.IsQuestComplete(Id);
            Expansion = questInfo.Expansion.Value.Name.ExtractText();
            Dalamud.Log.Verbose($"Quest: {Name} - Completed: {IsCompleted} - RepRow: {questInfo.BeastReputationRank.RowId}");
            if (!questInfo.BeastTribe.IsValid)
            {
                Dalamud.Log.Debug($"Quest: {Name} - No reputation required");
                return;
            }
            BeastReputationRank = questInfo.BeastReputationRank.RowId;
            BeastReputationValue = questInfo.BeastReputationValue == 65535 ? questInfo.BeastReputationRank.Value.RequiredReputation : questInfo.BeastReputationValue;
            BeastReputationRankName = questInfo.BeastReputationRank.Value.Name.ExtractText();
        }
    }
}
