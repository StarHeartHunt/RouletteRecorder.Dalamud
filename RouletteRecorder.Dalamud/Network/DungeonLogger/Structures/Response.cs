using Newtonsoft.Json;

namespace RouletteRecorder.Dalamud.Network.DungeonLogger.Structures
{
    public class Response<T>
    {
        [JsonProperty("code")]
        public required int Code { get; set; }

        [JsonProperty("data")]
        public T? Data { get; set; }

        [JsonProperty("msg")]
        public required string Msg { get; set; }
    }

    public class ErrorResponse
    {
        [JsonProperty("statusCode")]
        public required int Code { get; set; }

        [JsonProperty("message")]
        public required string[] Msg { get; set; }

        [JsonProperty("error")]
        public required string Error { get; set; }
    }
}
