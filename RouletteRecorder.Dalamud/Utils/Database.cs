using CsvHelper;
using Dalamud.Utility;
using Lumina.Excel.Sheets;
using Newtonsoft.Json;
using RouletteRecorder.Dalamud.DAO;
using RouletteRecorder.Dalamud.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace RouletteRecorder.Dalamud.Utils;

public class Database
{
    public static readonly string DbPath = Path.Combine(Plugin.PluginInterface.ConfigDirectory.FullName, "data.json");
    public static readonly string PendingDbPath = Path.Combine(Plugin.PluginInterface.ConfigDirectory.FullName, "data_pending.json");
    public static readonly ContentRoulette[] CfRoulettes = Plugin.DataManager.GetExcelSheet<ContentRoulette>().Where(roulette => roulette is { IsInDutyFinder: true, IsGoldSaucer: false }).ToArray();
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
        Roulettes.Add(roulette);
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
        return content.IsNullOrEmpty() ? null : JsonConvert.DeserializeObject<Roulette>(content);
    }

    public static void SavePendingRoulette()
    {
        if (Roulette.Instance != null)
        {
            File.WriteAllText(PendingDbPath, JsonConvert.SerializeObject(Roulette.Instance));
        }
    }

    public static void ExportAsCsv(string destPath)
    {
        // make excel recognize the encoding
        using var writer = new StreamWriter(destPath, false, new UTF8Encoding(true));
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        csv.Context.RegisterClassMap<RouletteCsvMap>();
        csv.WriteRecords(Roulettes);

        // open the file explorer to the export location
        var argument = "/select, \"" + destPath + "\"";
        Process.Start("explorer.exe", argument);
    }
}
