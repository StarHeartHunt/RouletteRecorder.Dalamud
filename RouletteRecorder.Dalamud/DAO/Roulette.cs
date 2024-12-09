using RouletteRecorder.Dalamud.Utils;
using System;
using System.Linq;

namespace RouletteRecorder.Dalamud.DAO;

public class Roulette(Plugin plugin, string? contentName, string? rouletteType, bool isCompleted = false)
{
    private readonly Configuration configuration = plugin.Configuration;
    public string? RouletteType { get; set; } = rouletteType;
    public string Date { get; set; } = DateTime.Now.ToString("yyyy-MM-dd");
    public string StartedAt { get; set; } = DateTime.Now.ToString("T");
    public string? EndedAt { get; set; } = null;
    public string? ContentName { get; set; } = contentName;
    public string? JobName { get; set; }
    public bool IsCompleted { get; set; } = isCompleted;
    public static Roulette? Instance { get; private set; }

    public static void Init(Plugin plugin, string? contentName = null, string? rouletteType = null, bool isCompleted = false)
    {
        Instance = new Roulette(plugin, contentName, rouletteType, isCompleted);
    }

    public static void Init(Roulette instance)
    {
        Instance = instance;
    }

    public void Finish()
    {
        if (Instance == null) return;

        var currContentRoulette = Database.CfRoulettes.FirstOrDefault(x => x.Name.ToString().Equals(RouletteType));
        var isSubscribedRouletteType = currContentRoulette != null && configuration.subscribedRouletteIds.Contains(currContentRoulette.RowId);
        if (Instance.RouletteType == null || Instance.ContentName == null || !isSubscribedRouletteType) return;

        Instance.JobName = Plugin.GetJobName() ?? "未知职业";
        Instance.EndedAt = DateTime.Now.ToString("T");

        Database.InsertRoulette(Instance);
        //if (Instance.IsCompleted) await UploadDungeonLogger();

        Instance = null;
    }
}
