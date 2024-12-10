using Dalamud.Configuration;
using Dalamud.Game;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.IO;

namespace RouletteRecorder.Dalamud;

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
    public string CSVExportPath { get; set; } = Path.Combine(Plugin.PluginInterface.ConfigDirectory.FullName, "data.csv");
    public bool IsConfigWindowMovable { get; set; } = true;

    public bool SetSubscribedRouletteId(ContentRoulette roulette, bool selected)
    {
        return selected ? SubscribedRouletteIds.Add(roulette.RowId) : SubscribedRouletteIds.Remove(roulette.RowId);
    }

    public void Save()
    {
        Plugin.PluginInterface.SavePluginConfig(this);
    }
}
