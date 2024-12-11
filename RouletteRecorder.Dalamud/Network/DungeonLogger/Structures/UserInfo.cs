using Newtonsoft.Json;

namespace RouletteRecorder.Dalamud.Network.DungeonLogger.Structures
{
    public class UserInfo
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("username")]
        public required string Username { get; set; }

        [JsonProperty("nickname")]
        public required string Nickname { get; set; }

        [JsonProperty("admin")]
        public bool Admin { get; set; }

        [JsonProperty("target")]
        public int Target { get; set; }
    }
}
