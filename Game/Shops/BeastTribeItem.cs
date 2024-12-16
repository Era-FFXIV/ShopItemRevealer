using Newtonsoft.Json;
using ShopItemRevealer.Game.Player;

namespace ShopItemRevealer.Game.Shops
{
    internal class BeastTribeItem : IGameInfo
    {
        internal BeastTribe BeastTribe { get; private set; }
        internal Quest? Quest { get; private set; }
        internal uint RequiredReputation { get; private set; } = 0;
        public uint Id => 0;
        public string Name => "";

        internal BeastTribeItem(BeastTribe beastTribe, Quest? quest)
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
