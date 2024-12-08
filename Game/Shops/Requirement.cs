using ShopItemRevealer.Game.Player;

namespace ShopItemRevealer.Game.Shops
{
    public enum LockedReasonType
    {
        Achievement,
        Quest,
        BeastTribe,
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
                ReputationValue = ReputationManager.CalculateReputation(item.Quest!.BeastReputationRank, item.Quest.BeastReputationValue);
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
                case LockedReasonType.None:
                    return true;
                default:
                    return false;
            }
        }
        public int NeededQuests() { 
            if (ReasonType != LockedReasonType.BeastTribe) { return 0; }
            BeastTribeItem tribe = (BeastTribeItem)RequirementObject;
            NeededQuestsInt = PlayerManager.GetQuestsNeededForRank(tribe.BeastTribe, tribe.RequiredReputation);
            return NeededQuestsInt;
        }
        public class NullRequirement : IGameInfo
        {
            public uint Id => 0;
            public string Name => "None";
        }
    }
}
