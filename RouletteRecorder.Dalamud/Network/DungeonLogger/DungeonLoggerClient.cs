using Newtonsoft.Json;
using RouletteRecorder.Dalamud.Network.DungeonLogger.Structures;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Plugin.Services;
using System.Collections;
using System.Linq;

namespace RouletteRecorder.Dalamud.Network.DungeonLogger
{
    public sealed class DungeonLoggerClient : IDisposable
    {
        private readonly HttpClient client;
        private readonly CookieContainer cookieContainer;

        public DungeonLoggerClient()
        {
            cookieContainer = new CookieContainer();
            client = new HttpClient(new HttpClientHandler()
            {
                CookieContainer = cookieContainer
            })
            {
                BaseAddress = new Uri("https://dlog.luyulight.cn"),
            };
        }

        public async Task<Response<object>?> PostLogin(string password, string username)
        {
            var response = await client.PostAsync("/api/login", new StringContent(JsonConvert.SerializeObject(new
            {
                password,
                username
            }), Encoding.UTF8, "application/json"));

            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                var obj = JsonConvert.DeserializeObject<ErrorResponse>(content)!;
                return new Response<object>()
                {
                    Data = { },
                    Code = obj.Code,
                    Msg = string.Join(",", obj.Msg)
                };
            }

            return JsonConvert.DeserializeObject<Response<object>>(content);
        }

        public async Task<object?> PostRecord(int mazeId, string profKey)
        {
            var response = await client.PostAsync("/api/record", new StringContent(JsonConvert.SerializeObject(new
            {
                mazeId,
                profKey
            }), Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject(content);
        }

        public async Task<Response<List<StatProf>>?> GetStatProf()
        {
            var response = await client.GetAsync("/api/stat/prof");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<Response<List<StatProf>>>(content);
        }

        public async Task<Response<List<StatMaze>>?> GetStatMaze()
        {
            var response = await client.GetAsync("/api/stat/maze");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<Response<List<StatMaze>>>(content);
        }

        public void Dispose()
        {
            client.Dispose();
        }
    }
}
