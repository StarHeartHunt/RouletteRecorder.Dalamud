using Dalamud.Configuration;
using Dalamud.Game;
using Lumina.Excel.GeneratedSheets;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace RouletteRecorder.Dalamud;

[Serializable]
public class DungeonLoggerConfig
{
    public bool Enabled = false;

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public string Username = string.Empty;

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public string Password = string.Empty;
}


[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 0;
    public string Language = Plugin.ClientState.ClientLanguage switch
    {
        ClientLanguage.English => "en",
        ClientLanguage.Japanese => "en",
        ClientLanguage.French => "en",
        ClientLanguage.German => "en",
        // ClientLanguage.ChineseSimplified only exist in CN ver of dalamud
        // ClientLanguage.ChineseSimplified => "zh_CN",
        _ => "zh_CN",
    };
    public HashSet<uint> SubscribedRouletteIds { get; set; } = [];
    public string CsvExportPath { get; set; } = Path.Combine(Plugin.PluginInterface.ConfigDirectory.FullName, "data.csv");
    public DungeonLoggerConfig DungeonLoggerConfig { get; set; } = new();

    public bool SetSubscribedRouletteId(ContentRoulette roulette, bool selected)
    {
        var ret = selected ? SubscribedRouletteIds.Add(roulette.RowId) : SubscribedRouletteIds.Remove(roulette.RowId);
        Save();

        return ret;
    }

    public void Save()
    {
        Plugin.PluginInterface.SavePluginConfig(this);
    }
}
