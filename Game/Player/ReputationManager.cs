using FFXIVClientStructs.FFXIV.Client.Game;
using Newtonsoft.Json;

namespace ShopItemRevealer.Game.Player
{
    internal static class ReputationManager
    {
        internal class Reputation(BeastTribe tribe, int rank, uint value)
        {
            public BeastTribe BeastTribe { get; set; } = tribe ?? new BeastTribe(0);
            public int Rank { get; set; } = rank;
            public uint Value { get; set; } = value;
        }
        internal static List<uint> CumulativeReputation { get; set; } = [];
        internal static List<Reputation> CurrentReputation { get; set; } = [];
        internal static void Initialize()
        {
            uint c = 0;
            for (uint i = 0; i < SheetManager.BeastReputationRankSheet.Count; i++)
            {
                c += SheetManager.BeastReputationRankSheet.GetRow(i).RequiredReputation;
                CumulativeReputation.Add(c);
                Dalamud.Log.Verbose($"Rank {i} - Required: {SheetManager.BeastReputationRankSheet.GetRow(i).RequiredReputation} - Cumulative: {c}");
            }
            ScanAllReputations();
        }
        internal static void ScanAllReputations()
        {
            List<Reputation> scan = [];
            var repDict = new List<KeyValuePair<BeastTribe, uint>>();
            for (uint i = 1; i < SheetManager.BeastTribeSheet.Count; i++)
            {
                var tribe = PlayerManager.GetBeastTribe(i);
                unsafe
                {
                    var rep = QuestManager.Instance()->GetBeastReputationById(tribe.Id);
                    Dalamud.Log.Debug($"Scanning reputation for tribe {tribe.Name} with rank {rep->Rank & 0x7F} and value {rep->Value}");
                    var calculatedRep = CalculateReputation(rep->Rank & 0x7F, rep->Value);
                    scan.Add(new Reputation(tribe, rep->Rank & 0x7F, calculatedRep));
                }
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
