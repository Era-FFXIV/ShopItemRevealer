using static ShopItemRevealer.Game.Player.ReputationManager;

namespace ShopItemRevealer.Game.Player
{
    internal class PlayerManager : IManager
    {
        internal static List<Achievement> Achievements { get; private set; } = [];
        internal static List<Quest> Quests { get; private set; } = [];
        internal static List<BeastTribe> BeastTribes { get; private set; } = [];
        internal static List<FateRank> FateRanks { get; private set; } = [];
        internal static bool HasFateRanksInitialized => FateRanks.Count > 0;
        private static ulong CharacterId { get; set; } = 0;

        public void Initialize(ShopItemRevealer plugin)
        {
            FateRanks = FateRank.FromJson();
            Dalamud.ClientState.Login += OnLogin;
            Dalamud.ClientState.Logout += OnLogout;
        }
        public void Dispose()
        {
            FateRanks = [];
            Quests = [];
            Achievements = [];
            BeastTribes = [];
            Dalamud.ClientState.Login -= OnLogin;
            Dalamud.ClientState.Logout -= OnLogout;
        }
        private void OnLogin()
        {
            CharacterId = Dalamud.ClientState.LocalContentId;
            FateRanks = FateRank.FromJson();
        }
        private void OnLogout(int _, int __)
        {
            FateRank.Save(FateRanks, CharacterId);
            FateRanks = [];
            Quests = [];
            Achievements = [];
            BeastTribes = [];
        }

        internal static void AddAchievement(Achievement achievement)
        {
            if (Achievements.Contains(achievement))
            {
                return;
            }
            Achievements.Add(achievement);
        }
        internal static Achievement GetAchievement(uint id)
        {
            var a = Achievements.Find(x => x.Id == id);
            if (a == null)
            {
                a = new Achievement(id);
                Achievements.Add(a);
            }
            return a;
        }
        internal static void AddQuest(Quest quest)
        {
            if (Quests.Contains(quest))
            {
                return;
            }
            Quests.Add(quest);
        }
        internal static Quest GetQuest(uint id)
        {
            var q = Quests.Find(x => x.Id == id);
            if (q == null)
            {
                q = new Quest(id);
                Quests.Add(q);
            }
            return q;
        }
        internal static void AddBeastTribe(BeastTribe beastTribe)
        {
            if (BeastTribes.Contains(beastTribe))
            {
                return;
            }
            BeastTribes.Add(beastTribe);
        }
        internal static BeastTribe GetBeastTribe(uint id)
        {
            var t = BeastTribes.Find(x => x.Id == id);
            if (t == null)
            {
                t = new BeastTribe(id);
                BeastTribes.Add(t);
            }
            return t;
        }
        internal static int GetQuestsNeededForRank(BeastTribe tribe, uint targetReputation)
        {
            var quests = SheetManager.QuestSheet.Where(x => x.BeastTribe.RowId == tribe.Id).ToList();
            var currentRank = GetReputation(tribe.Id);
            double neededReputation = targetReputation - currentRank.Value;
            // for each rank, determine the quests needed to rank up
            int questCount = 0;
            for (var i = currentRank.Rank; i < tribe.MaxRank; i++)
            {
                var rep = SheetManager.BeastReputationRankSheet.GetRow((uint)i);
                var questPool = quests.Where(x => x.BeastReputationRank.RowId == i).ToList();
                double repGiven = questPool.Max(x => x.ReputationReward);
                // if 0 rep given, use previous rank's rep given
                if (repGiven == 0)
                {
                    questPool = quests.Where(x => x.BeastReputationRank.RowId == i - 1).ToList();
                    repGiven = questPool.Max(x => x.ReputationReward);
                }
                double maxThisLevel = rep.RequiredReputation;
                Dalamud.Log.Verbose($"[GetQuestsNeededForRank()] Tribe {tribe.Name} - Rank {i} - Required: {neededReputation} - RepGiven: {repGiven} - Max this level: {maxThisLevel}");
                if (maxThisLevel < neededReputation)
                {
                    questCount += (int)Math.Ceiling(maxThisLevel / repGiven);
                    neededReputation -= maxThisLevel;
                    Dalamud.Log.Verbose($"[GetQuestsNeededForRank()] Tribe {tribe.Name} - Rank {i} - Quests: {questCount} - Calculation: {maxThisLevel}/{repGiven} - Remaining: {neededReputation}");
                    continue;
                } else
                {
                    questCount += (int)Math.Ceiling(neededReputation / repGiven);
                    Dalamud.Log.Verbose($"[GetQuestsNeededForRank()] Tribe {tribe.Name} - Rank {i} - Quests: {questCount} - Final Calculated: {neededReputation}/{repGiven}");
                    break;
                }
            }
            Dalamud.Log.Verbose($"[GetQuestsNeededForRank()] Tribe {tribe.Name} - Total Quests: {questCount}");
            return questCount;
        }
        internal static FateRank? GetFateRank(uint id)
        {
            var f = FateRanks.Find(x => x.TerritoryId == id);
            return f;
        }
        internal static void AddFateRank(uint TerritoryId, uint Rank)
        {
            var rank = FateRanks.FirstOrDefault(x => x.TerritoryId == TerritoryId);
            if (rank != null)
            {
                rank.Rank = Rank;
            } else
            {
                FateRanks.Add(new FateRank(TerritoryId, Rank));
            }
        }
    }
}
