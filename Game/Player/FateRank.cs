using Newtonsoft.Json;

namespace ShopItemRevealer.Game.Player
{
    [Serializable]
    internal class FateRank : IGameInfo
    {
        public ulong CharacterId { get; init; }
        public uint TerritoryId { get; init; }
        public uint Rank { get; init; }
        public string ZoneName { get; init; }

        public uint Id => TerritoryId;
        public string Name => ZoneName;
        public FateRank(uint territoryId, uint rank, string zoneName)
        {
            TerritoryId = territoryId;
            Rank = rank;
            ZoneName = zoneName;
            CharacterId = Dalamud.ClientState.LocalContentId;
        }
        [JsonConstructor]
        public FateRank(uint territoryId, uint rank, string zoneName, ulong characterId)
        {
            TerritoryId = territoryId;
            Rank = rank;
            ZoneName = zoneName;
            CharacterId = characterId;
        }
        public static List<FateRank> FromJson()
        {
            var json = Path.Combine(Dalamud.PluginInterface.GetPluginConfigDirectory(), "FateRanks.json");
            if (!File.Exists(json))
            {
                return [];
            }
            var data = File.ReadAllText(json);
            var ranks = JsonConvert.DeserializeObject<List<FateRank>>(data);
            if (ranks == null)
            {
                return [];
            }
            return ranks;
        }
        public static void Save(List<FateRank> ranks)
        {
            var json = Path.Combine(Dalamud.PluginInterface.GetPluginConfigDirectory(), "FateRanks.json");
            File.WriteAllText(json, JsonConvert.SerializeObject(ranks, Formatting.Indented));
        }
    }
}