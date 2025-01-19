using Lumina.Extensions;
using ShopItemRevealer.Game.Player;

namespace ShopItemRevealer.Game.Shops
{
    public enum LockedReasonType
    {
        Achievement,
        Quest,
        BeastTribe,
        FateRank,
        None
    }
    internal class Requirement
    {
        public LockedReasonType ReasonType { get; init; }
        internal IGameInfo RequirementObject { get; init; }
        internal uint ReputationValue { get; init; }
        internal int NeededQuestsInt { get; private set; }
        public override string ToString()
        {
            switch (ReasonType)
            {
                case LockedReasonType.Achievement:
                    return $"Achievement {RequirementObject.Name}";
                case LockedReasonType.Quest:
                    return $"Quest: {RequirementObject.Name}";
                case LockedReasonType.BeastTribe:
                    BeastTribeItem tribe = (BeastTribeItem)RequirementObject;
                    var currentRep = ReputationManager.GetReputation(tribe.BeastTribe.Id).Value;
                    if (tribe.RequiredReputation == 0) return "Allied Society Quest";
                    return $"Reputation: {tribe.Quest!.BeastReputationRankName} ({currentRep}/{tribe.RequiredReputation})";
                case LockedReasonType.FateRank:
                    FateShopItem fateRank = (FateShopItem)RequirementObject;
                    var currentRank = PlayerManager.GetFateRank(fateRank.Item.IsAllItems ? Dalamud.ClientState.TerritoryType : fateRank.TerritoryId);
                    var zoneName = fateRank.Item.ZoneName;
                    if (fateRank.Item.IsAllItems) zoneName = SheetManager.TerritoryTypeSheet.GetRow(Dalamud.ClientState.TerritoryType).PlaceName.Value.Name.ExtractText();
                    if (currentRank == null) return $"Shared FATE Rank ({zoneName}): {fateRank.Item.RankRequired} - Yours: 0";
                    return $"Shared FATE Rank ({zoneName}): {fateRank.Item.RankRequired} - Yours: {currentRank.Rank}";
                case LockedReasonType.None:
                    return $"None";
                default:
                    return "Requires something";
            }
        }
        public Requirement(LockedReasonType reasonType, IGameInfo requirement)
        {
            ReasonType = reasonType;
            RequirementObject = requirement;
            if (requirement.GetType() == typeof(BeastTribeItem)) 
            {
                var item = (BeastTribeItem)RequirementObject;
                ReputationValue = ReputationManager.CalculateReputation((int)item.Quest!.BeastReputationRank, item.Quest.BeastReputationValue);
            }
            else if (requirement.GetType() == typeof(FateShopItem))
            {
                var item = (FateShopItem)RequirementObject;
                var quest = SheetManager.QuestSheet.FirstOrNull(x => x.RowId == item.Item.QuestId);
                if (quest != null)
                {
                    NeededQuestsInt = (int)quest.Value.RowId;
                }
            }
        }
        public bool MeetsRequirement()
        {
            switch (ReasonType)
            {
                case LockedReasonType.Achievement:
                    var a = (Achievement)RequirementObject;
                    return a.IsComplete() == CompletionStatus.Complete;
                case LockedReasonType.Quest:
                    var q = (Quest)RequirementObject;
                    return q.IsCompleted;
                case LockedReasonType.BeastTribe:
                    BeastTribeItem tribe = (BeastTribeItem)RequirementObject;
                    return ReputationManager.GetReputation(tribe.BeastTribe.Id).Value >= tribe.RequiredReputation;
                case LockedReasonType.FateRank:
                    FateShopItem fateRank = (FateShopItem)RequirementObject;
                    var territoryId = fateRank.TerritoryId == 0 ? Dalamud.ClientState.TerritoryType : fateRank.TerritoryId;
                    if (PlayerManager.GetFateRank(territoryId) != null)
                    {
                        return PlayerManager.GetFateRank(territoryId)!.Rank >= fateRank.Item.RankRequired;
                    }
                    else { return false; }
                case LockedReasonType.None:
                    return true;
                default:
                    return false;
            }
        }
        public int NeededQuests()
        {
            if (ReasonType == LockedReasonType.BeastTribe)
            {
                BeastTribeItem tribe = (BeastTribeItem)RequirementObject;
                NeededQuestsInt = PlayerManager.GetQuestsNeededForRank(tribe.BeastTribe, tribe.RequiredReputation);
                return NeededQuestsInt;
            }
            else if (ReasonType == LockedReasonType.FateRank)
            {
                return NeededQuestsInt;
            }
            return 0;
        }
        public class NullRequirement : IGameInfo
        {
            public uint Id => 0;
            public string Name => "None";
        }
    }
}
