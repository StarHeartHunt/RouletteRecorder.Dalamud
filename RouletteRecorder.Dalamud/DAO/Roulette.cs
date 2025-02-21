using Dalamud.Utility;
using RouletteRecorder.Dalamud.Network.DungeonLogger;
using RouletteRecorder.Dalamud.Utils;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RouletteRecorder.Dalamud.DAO;

public class Roulette(string? contentName, string? rouletteType, bool isCompleted = false)
{
    public string? RouletteType { get; set; } = rouletteType;
    public string Date { get; set; } = DateTime.Now.ToString("yyyy-MM-dd");
    public string StartedAt { get; set; } = DateTime.Now.ToString("T");
    public string? EndedAt { get; set; }
    public string? ContentName { get; set; } = contentName;
    public string? JobName { get; set; }
    public bool IsCompleted { get; set; } = isCompleted;
    public static Roulette? Instance { get; private set; }

    public static void Init(string? contentName = null, string? rouletteType = null, bool isCompleted = false)
    {
        Instance = new Roulette(contentName, rouletteType, isCompleted);
    }

    public static void Init(Roulette instance)
    {
        Instance = instance;
    }

    public async void Finish()
    {
        try
        {
            if (Instance == null) return;

            var currContentRoulette = Database.CfRoulettes.FirstOrDefault(x => x.Name.ToString().Equals(RouletteType));
            var isSubscribedRouletteType = Plugin.Configuration.SubscribedRouletteIds.Contains(currContentRoulette.RowId);
            if (Instance.RouletteType == null || Instance.ContentName == null || !isSubscribedRouletteType) return;

            Instance.JobName = Plugin.GetJobName() ?? "未知职业";
            Instance.EndedAt = DateTime.Now.ToString("T");

            Database.InsertRoulette(Instance);
            if (Instance.IsCompleted && Plugin.Configuration.DungeonLoggerConfig.Enabled) await UploadDungeonLogger();

            Instance = null;
        }
        catch (Exception e)
        {
            Plugin.PluginLog.Error(e, "Failed to finish roulette");
        }
    }

    public static async Task UploadDungeonLogger()
    {
        try
        {
            if (Instance == null)
            {
                Plugin.PluginLog.Debug("null Roulette Instance when uploading to DungeonLogger");
                return;
            }

            var username = Plugin.Configuration.DungeonLoggerConfig.Username;
            var password = Plugin.Configuration.DungeonLoggerConfig.Password;
            if (username.IsNullOrEmpty() || password.IsNullOrEmpty())
            {
                Plugin.PluginLog.Warning("DungeonLogger enabled but username or password is empty");
                return;
            }

            using var client = new DungeonLoggerClient();
            await client.PostLogin(password, username);

            var maze = await client.GetStatMaze();
            if (maze?.Data == null) throw new Exception("maze data from DungeonLogger is null");

            var job = await client.GetStatProf();
            if (job?.Data == null) throw new Exception("job data from DungeonLogger is null");

            var mazeId = maze.Data.Find(ele => ele.Name.Equals(Instance.ContentName))?.Id ??
                         throw new Exception("cannot convert to DungeonLogger mazeId");
            var profKey = job.Data.Find(ele => ele.NameCn.Equals(Instance.JobName))?.Key ??
                          throw new Exception("cannot convert to DungeonLogger profKey");
            await client.PostRecord(mazeId, profKey);
        }
        catch (Exception e)
        {
            Plugin.PluginLog.Error(e, "Failed to upload roulette result to DungeonLogger");
        }
    }
}
