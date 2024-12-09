using Dalamud.Utility;
using Lumina.Excel.GeneratedSheets;
using Newtonsoft.Json;
using RouletteRecorder.Dalamud.DAO;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RouletteRecorder.Dalamud.Utils;

public class Database
{
    public static readonly string DbPath = Path.Combine(Plugin.PluginInterface.ConfigDirectory.FullName, "data.json");
    public static readonly string PendingDbPath = Path.Combine(Plugin.PluginInterface.ConfigDirectory.FullName, "data_pending.json");
    public static readonly ContentRoulette[] CfRoulettes = Plugin.DataManager.GetExcelSheet<ContentRoulette>()?.Where(roulette => roulette.IsInDutyFinder && !roulette.IsGoldSaucer)?.ToArray() ?? [];
    public static bool IsPendingDbExists() => File.Exists(PendingDbPath);

    public static List<Roulette> Roulettes { get; private set; } = [];

    public static void Load()
    {
        if (!File.Exists(DbPath)) Save();

        var content = File.ReadAllText(DbPath);

        var deserialized = JsonConvert.DeserializeObject<List<Roulette>>(content);
        if (deserialized != null)
        {
            Roulettes = deserialized;
        }
    }

    public static void InsertRoulette(Roulette roulette)
    {
        Roulettes?.Add(roulette);
        Save();
    }

    public static void Save()
    {
        File.WriteAllText(DbPath, JsonConvert.SerializeObject(Roulettes));
    }

    public static Roulette? LoadFromPendingData()
    {
        if (!File.Exists(PendingDbPath)) return null;

        var content = File.ReadAllText(DbPath);
        if (content.IsNullOrEmpty()) return null;

        return JsonConvert.DeserializeObject<Roulette>(content);
    }

    public static void SavePendingRoulette()
    {
        if (Roulette.Instance != null)
        {
            File.WriteAllText(PendingDbPath, JsonConvert.SerializeObject(Roulette.Instance));
        }
    }
}
