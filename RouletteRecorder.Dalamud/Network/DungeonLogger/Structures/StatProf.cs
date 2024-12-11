using Newtonsoft.Json;

namespace RouletteRecorder.Dalamud.Network.DungeonLogger.Structures
{
    public class StatProf
    {
        [JsonProperty("key")]
        public required string Key { get; set; }

        [JsonProperty("nameCn")]
        public required string NameCn { get; set; }

        [JsonProperty("type")]
        public required string Type { get; set; }
    }
}
