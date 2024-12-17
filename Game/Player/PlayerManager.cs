using static ShopItemRevealer.Game.Player.ReputationManager;

namespace ShopItemRevealer.Game.Player
{
    /// <summary>
    /// Manages player-related data and operations.
    /// </summary>
    public static class PlayerManager
    {
        /// <summary>
        /// Gets the list of achievements.
        /// </summary>
        public static List<Achievement> Achievements { get; private set; } = [];

        /// <summary>
        /// Gets the list of quests.
        /// </summary>
        public static List<Quest> Quests { get; private set; } = [];

        /// <summary>
        /// Gets the list of beast tribes.
        /// </summary>
        public static List<BeastTribe> BeastTribes { get; private set; } = [];

        /// <summary>
        /// Adds an achievement to the list.
        /// </summary>
        /// <param name="achievement">The achievement to add.</param>
        public static void AddAchievement(Achievement achievement)
        {
            if (Achievements.Contains(achievement))
            {
                return;
            }
            Achievements.Add(achievement);
        }

        /// <summary>
        /// Gets an achievement by its ID.
        /// </summary>
        /// <param name="id">The ID of the achievement.</param>
        /// <returns>The achievement with the specified ID.</returns>
        public static Achievement GetAchievement(uint id)
        {
            var achievement = Achievements.Find(x => x.Id == id);
            if (achievement == null)
            {
                achievement = new Achievement(id);
                Achievements.Add(achievement);
            }
            return achievement;
        }

        /// <summary>
        /// Adds a quest to the list.
        /// </summary>
        /// <param name="quest">The quest to add.</param>
        public static void AddQuest(Quest quest)
        {
            if (Quests.Contains(quest))
            {
                return;
            }
            Quests.Add(quest);
        }

        /// <summary>
        /// Gets a quest by its ID.
        /// </summary>
        /// <param name="id">The ID of the quest.</param>
        /// <returns>The quest with the specified ID.</returns>
        public static Quest GetQuest(uint id)
        {
            var quest = Quests.Find(x => x.Id == id);
            if (quest == null)
            {
                quest = new Quest(id);
                Quests.Add(quest);
            }
            return quest;
        }

        /// <summary>
        /// Adds a beast tribe to the list.
        /// </summary>
        /// <param name="beastTribe">The beast tribe to add.</param>
        public static void AddBeastTribe(BeastTribe beastTribe)
        {
            if (BeastTribes.Contains(beastTribe))
            {
                return;
            }
            BeastTribes.Add(beastTribe);
        }

        /// <summary>
        /// Gets a beast tribe by its ID.
        /// </summary>
        /// <param name="id">The ID of the beast tribe.</param>
        /// <returns>The beast tribe with the specified ID.</returns>
        public static BeastTribe GetBeastTribe(uint id)
        {
            var beastTribe = BeastTribes.Find(x => x.Id == id);
            if (beastTribe == null)
            {
                beastTribe = new BeastTribe(id);
                BeastTribes.Add(beastTribe);
            }
            return beastTribe;
        }

        /// <summary>
        /// Gets the number of quests needed to reach a target reputation for a beast tribe.
        /// </summary>
        /// <param name="tribe">The beast tribe.</param>
        /// <param name="targetReputation">The target reputation.</param>
        /// <returns>The number of quests needed to reach the target reputation.</returns>
        public static int GetQuestsNeededForRank(BeastTribe tribe, uint targetReputation)
        {
            var quests = SheetManager.QuestSheet.Where(x => x.BeastTribe.RowId == tribe.Id).ToList();
            var currentRank = GetReputation(tribe.Id);
            double neededReputation = targetReputation - currentRank.Value;
            int questCount = 0;
            for (var i = currentRank.Rank; i < tribe.MaxRank; i++)
            {
                var rep = SheetManager.BeastReputationRankSheet.GetRow((uint)i);
                var questPool = quests.Where(x => x.BeastReputationRank.RowId == i).ToList();
                double repGiven = questPool.Max(x => x.ReputationReward);
                if (repGiven == 0)
                {
                    questPool = quests.Where(x => x.BeastReputationRank.RowId == i - 1).ToList();
                    repGiven = questPool.Max(x => x.ReputationReward);
                }
                double maxThisLevel = rep.RequiredReputation;
                if (maxThisLevel < neededReputation)
                {
                    questCount += (int)Math.Ceiling(maxThisLevel / repGiven);
                    neededReputation -= maxThisLevel;
                    continue;
                }
                else
                {
                    questCount += (int)Math.Ceiling(neededReputation / repGiven);
                    break;
                }
            }
            return questCount;
        }
    }
}
