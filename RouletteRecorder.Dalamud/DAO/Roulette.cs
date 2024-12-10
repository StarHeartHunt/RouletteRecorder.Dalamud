using RouletteRecorder.Dalamud.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RouletteRecorder.Dalamud.DAO;

public class Roulette(string? contentName, string? rouletteType, bool isCompleted = false)
{
    public string? RouletteType { get; set; } = rouletteType;
    public string Date { get; set; } = DateTime.Now.ToString("yyyy-MM-dd");
    public string StartedAt { get; set; } = DateTime.Now.ToString("T");
    public string? EndedAt { get; set; } = null;
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

    public void Finish(HashSet<uint> subscribedRouletteIds)
    {
        if (Instance == null) return;

        var currContentRoulette = Database.CfRoulettes.FirstOrDefault(x => x.Name.ToString().Equals(RouletteType));
        var isSubscribedRouletteType = currContentRoulette != null && subscribedRouletteIds.Contains(currContentRoulette.RowId);
        if (Instance.RouletteType == null || Instance.ContentName == null || !isSubscribedRouletteType) return;

        Instance.JobName = Plugin.GetJobName() ?? "未知职业";
        Instance.EndedAt = DateTime.Now.ToString("T");

        Database.InsertRoulette(Instance);
        //if (Instance.IsCompleted) await UploadDungeonLogger();

        Instance = null;
    }
}
