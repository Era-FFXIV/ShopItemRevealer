using Newtonsoft.Json;

namespace ShopItemRevealer.Game.Player
{
    [Serializable]
    internal class FateRank : IGameInfo
    {
        public uint TerritoryId { get; init; }
        public uint Rank { get; set; }
        public string ZoneName { get; init; }
        public uint Id => TerritoryId;
        public string Name => ZoneName;

        [JsonConstructor]
        public FateRank(uint territoryId, uint rank)
        {
            TerritoryId = territoryId;
            Rank = rank;
            if (SheetManager.TerritoryTypeSheet.TryGetRow(territoryId, out var territory))
            {
                ZoneName = territory.PlaceName.Value.Name.ExtractText();
            }
            else
            {
                ZoneName = "Unknown";
                Dalamud.Log.Error($"[FateRank] Territory {territoryId} not found.");
            }
        }
        public static List<FateRank> FromJson()
        {
            if (Dalamud.ClientState.LocalPlayer == null)
            {
                Dalamud.Log.Debug("LocalPlayer is null");
                return [];
            }
            var json = Path.Combine(Dalamud.PluginInterface.GetPluginConfigDirectory(), $"FateRanks-{Dalamud.ClientState.LocalContentId}.json");
            if (!File.Exists(json))
            {
                Dalamud.Log.Debug($"File {json} does not exist");
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
        public static void Save(List<FateRank> ranks, ulong CharacterId)
        {
            var json = Path.Combine(Dalamud.PluginInterface.GetPluginConfigDirectory(), $"FateRanks-{CharacterId}.json");
            File.WriteAllText(json, JsonConvert.SerializeObject(ranks, Formatting.Indented));
        }
    }
}