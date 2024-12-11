using Newtonsoft.Json;

namespace RouletteRecorder.Dalamud.Network.DungeonLogger.Structures
{
    public class StatMaze
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public required string Name { get; set; }

        [JsonProperty("type")]
        public required string Type { get; set; }

        [JsonProperty("level")]
        public int Level { get; set; }
    }
}
