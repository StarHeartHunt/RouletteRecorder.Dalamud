using Newtonsoft.Json;
using RouletteRecorder.Dalamud.DAO;
using System.Collections.Generic;
using System.IO;

namespace RouletteRecorder.Dalamud.Utils;

public class Database
{
    public static readonly string DbPath = Path.Combine(Plugin.PluginInterface.ConfigDirectory.FullName, "data.json");

    private static List<Roulette> Roulettes = [];

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

    public static void Save()
    {
        File.WriteAllText(DbPath, JsonConvert.SerializeObject(Roulettes));
    }

    public static void InsertRoulette(Roulette roulette)
    {
        Roulettes?.Add(roulette);
        Save();
    }
}
