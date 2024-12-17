using FFXIVClientStructs.FFXIV.Client.Game;

namespace ShopItemRevealer.Game.Player
{
    /// <summary>
    /// Represents a quest in the game.
    /// </summary>
    public class Quest : IGameInfo
    {
        /// <summary>
        /// Gets the ID of the quest.
        /// </summary>
        public uint Id { get; private set; }

        /// <summary>
        /// Gets the name of the quest.
        /// </summary>
        public string Name { get; init; } = null!;

        /// <summary>
        /// Gets a value indicating whether the quest is completed.
        /// </summary>
        public bool IsCompleted { get; init; } = false;

        /// <summary>
        /// Gets the beast reputation rank required for the quest.
        /// </summary>
        public uint BeastReputationRank { get; init; } = 0;

        /// <summary>
        /// Gets the name of the beast reputation rank required for the quest.
        /// </summary>
        public string BeastReputationRankName { get; init; } = "None";

        /// <summary>
        /// Gets the beast reputation value required for the quest.
        /// </summary>
        public uint BeastReputationValue { get; init; } = 0;

        /// <summary>
        /// Gets the expansion associated with the quest.
        /// </summary>
        public string Expansion { get; init; } = "None";

        /// <summary>
        /// Returns a string representation of the quest.
        /// </summary>
        /// <returns>A string representing the quest.</returns>
        public override string ToString()
        {
            return $"Quest: {Name}";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Quest"/> class with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the quest.</param>
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
