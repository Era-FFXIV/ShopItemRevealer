using Newtonsoft.Json;
using ShopItemRevealer.Game.Player;

namespace ShopItemRevealer.Game.Shops
{
    /// <summary>
    /// Represents an item related to a beast tribe.
    /// </summary>
    public class BeastTribeItem : IGameInfo
    {
        /// <summary>
        /// Gets the beast tribe associated with the item.
        /// </summary>
        public BeastTribe BeastTribe { get; private set; }

        /// <summary>
        /// Gets the quest associated with the item.
        /// </summary>
        public Quest? Quest { get; private set; }

        /// <summary>
        /// Gets the required reputation for the item.
        /// </summary>
        public uint RequiredReputation { get; private set; } = 0;

        /// <summary>
        /// Gets the ID of the item.
        /// </summary>
        public uint Id => 0;

        /// <summary>
        /// Gets the name of the item.
        /// </summary>
        public string Name => "";

        /// <summary>
        /// Initializes a new instance of the <see cref="BeastTribeItem"/> class.
        /// </summary>
        /// <param name="beastTribe">The beast tribe associated with the item.</param>
        /// <param name="quest">The quest associated with the item.</param>
        public BeastTribeItem(BeastTribe beastTribe, Quest? quest)
        {
            BeastTribe = beastTribe;
            Quest = quest;
            if (quest != null)
            {
                Dalamud.Log.Verbose($"BeastTribeItem: {BeastTribe.Name} - Quest: {quest.Name}");
                Dalamud.Log.Verbose(JsonConvert.SerializeObject(quest));
                RequiredReputation = ReputationManager.CalculateReputation((int)quest.BeastReputationRank, quest.BeastReputationValue);
            }
        }
    }
}
