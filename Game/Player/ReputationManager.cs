using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Newtonsoft.Json;

namespace ShopItemRevealer.Game.Player
{
    internal static class ReputationManager
    {
        internal class Reputation(BeastTribe tribe, int rank, uint value)
        {
            public BeastTribe BeastTribe { get; set; } = tribe ?? new BeastTribe(0);
            public int Rank { get; set; } = rank;
            public uint ReputationValue { get; set; } = CalculateReputation(rank, value);
        }
        internal static List<uint> CumulativeReputation { get; set; } = [];
        internal static List<Reputation> CurrentReputation { get; set; } = [];
        internal static void Initialize()
        {
            ClientState.Login += OnLogin;
            ClientState.Logout += OnLogout;
            uint c = 0;
            for (uint i = 0; i < SheetManager.BeastReputationRankSheet.Count; i++)
            {
                c += SheetManager.BeastReputationRankSheet.GetRow(i).RequiredReputation;
                CumulativeReputation.Add(c);
                Dalamud.Log.Verbose($"Rank {i} - Required: {SheetManager.BeastReputationRankSheet.GetRow(i).RequiredReputation} - Cumulative: {c}");
            }
            ScanAllReputations();
        }

        public static void Dispose()
        {
            ClientState.Login -= OnLogin;
            ClientState.Logout -= OnLogout;
        }

        private static void OnLogout(int type, int code)
        {
            CumulativeReputation = [];
            CurrentReputation = [];
        }

        internal static void OnLogin() 
        {
            ScanAllReputations();
        }

        internal unsafe static void ScanAllReputations()
        {
            if (!PlayerState.Instance()->IsLoaded)
            {
                Log.Debug("[ReputationManager] PlayerState is not initialized, cannot scan reputations.");
                return;
            }
            List<Reputation> scan = [];
            var repDict = new List<KeyValuePair<BeastTribe, uint>>();
            for (uint i = 1; i < SheetManager.BeastTribeSheet.Count; i++)
            {
                var tribe = PlayerManager.GetBeastTribe(i);
                var rank = PlayerState.Instance()->GetBeastTribeRank((byte)i);
                var rep = PlayerState.Instance()->GetBeastTribeCurrentReputation((byte)i);
                Dalamud.Log.Debug($"Scanning reputation for tribe {tribe.Name} with rank {rank} and value {rep}");
                var calculatedRep = CalculateReputation(rank, rep);
                Log.Debug($"Calculated reputation for tribe {tribe.Name}: {calculatedRep}");
                scan.Add(new Reputation(tribe, rank, rep));
            }
            CurrentReputation = scan;
            Dalamud.Log.Verbose($"[ReputationManager] Scanned {CurrentReputation.Count} reputations: {JsonConvert.SerializeObject(CurrentReputation)}");
        }
        internal static Reputation GetReputation(uint beastTribeId)
        {
            if (CurrentReputation.Count == 0)
            {
                throw new InvalidDataException("Reputations have not been scanned yet, this shouldn't happen.");
            }
            if (beastTribeId == 0) return new Reputation(new BeastTribe(0), 0, 0);
            var r = CurrentReputation.First(CurrentReputation => CurrentReputation.BeastTribe.Id == beastTribeId);
            return r;
        }
        internal static uint CalculateReputation(int rank, uint reputation)
        {
            if (rank == 0) return 0;
            return CumulativeReputation[(int)rank-1] + reputation;
        }
    }
}
