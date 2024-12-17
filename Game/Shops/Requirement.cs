using ShopItemRevealer.Game.Player;

namespace ShopItemRevealer.Game.Shops
{
    /// <summary>
    /// Represents the type of reason for a locked requirement.
    /// </summary>
    public enum LockedReasonType
    {
        Achievement,
        Quest,
        BeastTribe,
        None
    }

    /// <summary>
    /// Represents a requirement for unlocking a shop item.
    /// </summary>
    public class Requirement
    {
        /// <summary>
        /// Gets the type of reason for the locked requirement.
        /// </summary>
        public LockedReasonType ReasonType { get; init; }

        /// <summary>
        /// Gets the object representing the requirement.
        /// </summary>
        public IGameInfo RequirementObject { get; init; }

        /// <summary>
        /// Gets the reputation value required for the requirement.
        /// </summary>
        public uint ReputationValue { get; init; }

        /// <summary>
        /// Gets the number of quests needed to meet the requirement.
        /// </summary>
        public int NeededQuestsInt { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Requirement"/> class with the specified reason type and requirement object.
        /// </summary>
        /// <param name="reasonType">The type of reason for the locked requirement.</param>
        /// <param name="requirement">The object representing the requirement.</param>
        public Requirement(LockedReasonType reasonType, IGameInfo requirement)
        {
            ReasonType = reasonType;
            RequirementObject = requirement;
            if (requirement.GetType() == typeof(BeastTribeItem))
            {
                var item = (BeastTribeItem)RequirementObject;
                ReputationValue = ReputationManager.CalculateReputation((int)item.Quest!.BeastReputationRank, item.Quest.BeastReputationValue);
            }
        }

        /// <summary>
        /// Determines whether the requirement is met.
        /// </summary>
        /// <returns><c>true</c> if the requirement is met; otherwise, <c>false</c>.</returns>
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

        /// <summary>
        /// Gets the number of quests needed to meet the requirement.
        /// </summary>
        /// <returns>The number of quests needed to meet the requirement.</returns>
        public int NeededQuests()
        {
            if (ReasonType != LockedReasonType.BeastTribe) { return 0; }
            BeastTribeItem tribe = (BeastTribeItem)RequirementObject;
            NeededQuestsInt = PlayerManager.GetQuestsNeededForRank(tribe.BeastTribe, tribe.RequiredReputation);
            return NeededQuestsInt;
        }

        /// <summary>
        /// Returns a string representation of the requirement.
        /// </summary>
        /// <returns>A string representing the requirement.</returns>
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

        /// <summary>
        /// Represents a null requirement.
        /// </summary>
        public class NullRequirement : IGameInfo
        {
            /// <summary>
            /// Gets the ID of the null requirement.
            /// </summary>
            public uint Id => 0;

            /// <summary>
            /// Gets the name of the null requirement.
            /// </summary>
            public string Name => "None";
        }
    }
}
