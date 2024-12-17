using FFXIVClientStructs.FFXIV.Client.Game;
using Newtonsoft.Json;

namespace ShopItemRevealer.Game.Player
{
    /// <summary>
    /// Manages the reputation for beast tribes.
    /// </summary>
    public static class ReputationManager
    {
        /// <summary>
        /// Represents the reputation of a beast tribe.
        /// </summary>
        public class Reputation
        {
            /// <summary>
            /// Gets or sets the beast tribe.
            /// </summary>
            public BeastTribe BeastTribe { get; set; }

            /// <summary>
            /// Gets or sets the rank of the beast tribe.
            /// </summary>
            public int Rank { get; set; }

            /// <summary>
            /// Gets or sets the reputation value of the beast tribe.
            /// </summary>
            public uint Value { get; set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Reputation"/> class.
            /// </summary>
            /// <param name="tribe">The beast tribe.</param>
            /// <param name="rank">The rank of the beast tribe.</param>
            /// <param name="value">The reputation value of the beast tribe.</param>
            public Reputation(BeastTribe tribe, int rank, uint value)
            {
                BeastTribe = tribe ?? new BeastTribe(0);
                Rank = rank;
                Value = value;
            }
        }

        /// <summary>
        /// Gets or sets the cumulative reputation values.
        /// </summary>
        public static List<uint> CumulativeReputation { get; set; } = new();

        /// <summary>
        /// Gets or sets the current reputation values.
        /// </summary>
        public static List<Reputation> CurrentReputation { get; set; } = new();

        /// <summary>
        /// Initializes the reputation manager.
        /// </summary>
        public static void Initialize()
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

        /// <summary>
        /// Scans all reputations.
        /// </summary>
        public static void ScanAllReputations()
        {
            List<Reputation> scan = new();
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

        /// <summary>
        /// Gets the reputation of a beast tribe.
        /// </summary>
        /// <param name="beastTribeId">The ID of the beast tribe.</param>
        /// <returns>The reputation of the beast tribe.</returns>
        public static Reputation GetReputation(uint beastTribeId)
        {
            if (CurrentReputation.Count == 0)
            {
                throw new InvalidDataException("Reputations have not been scanned yet, this shouldn't happen.");
            }
            if (beastTribeId == 0) return new Reputation(new BeastTribe(0), 0, 0);
            var r = CurrentReputation.First(CurrentReputation => CurrentReputation.BeastTribe.Id == beastTribeId);
            return r;
        }

        /// <summary>
        /// Calculates the reputation value.
        /// </summary>
        /// <param name="rank">The rank of the beast tribe.</param>
        /// <param name="reputation">The reputation value.</param>
        /// <returns>The calculated reputation value.</returns>
        public static uint CalculateReputation(int rank, uint reputation)
        {
            if (rank == 0) return 0;
            return CumulativeReputation[(int)rank - 1] + reputation;
        }
    }
}
