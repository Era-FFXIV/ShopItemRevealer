using Newtonsoft.Json;
using System.IO;

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
                Log.Error($"[FateRank] Territory {territoryId} not found.");
            }
        }
        public static List<FateRank> FromJson()
        {
            if (!PlayerState.IsLoaded)
            {
                return [];
            }
            var json = Path.Combine(PluginInterface.GetPluginConfigDirectory(), $"FateRanks-{PlayerState.ContentId}.json");
            if (!File.Exists(json))
            {
                Log.Debug($"File {json} does not exist");
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
            var json = Path.Combine(PluginInterface.GetPluginConfigDirectory(), $"FateRanks-{CharacterId}.json");
            File.WriteAllText(json, JsonConvert.SerializeObject(ranks, Formatting.Indented));
        }
    }
}